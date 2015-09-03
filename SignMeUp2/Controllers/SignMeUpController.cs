using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using SignMeUp2.Helpers;

namespace SignMeUp2.Controllers
{
    public class SignMeUpController : BaseController
    {
        protected override string GetEntitetsNamn()
        {
            return string.Empty;
        }

        public ActionResult Index(int? id)
        {
            SignMeUpVM SUPVM = (SignMeUpVM)Session["VM"];

            // Om view model innehåller ett evenemangsid som inte stämmer med inkommande id, nolla wizard
            if (SUPVM != null && id != null && SUPVM.EvenemangsId != id.Value)
            {
                SUPVM = null;
            }

            if (SUPVM == null)
            {
                if (id == null)
                    return ShowError(log, "Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.", false);

                var evenemang = smuService.Db.Evenemang.Find(id);

                LogDebug(log, "Användare går in på anmälan för: " + evenemang.Namn);

                var evenemangResult = EvenemangHelper.EvaluateEvenemang(evenemang);

                if (evenemangResult == EvenemangHelper.EvenemangValidationResult.NotOpen)
                {
                    ViewBag.Closed = false;
                    ViewBag.NotOpen = true;
                    return View("RegNotOpen", evenemang);
                }
                else if (evenemangResult == EvenemangHelper.EvenemangValidationResult.Closed)
                {
                    ViewBag.Closed = true;
                    ViewBag.NotOpen = false;
                    return View("RegNotOpen", evenemang);
                }
                else if (evenemangResult == EvenemangHelper.EvenemangValidationResult.DoesNotExist)
                {
                    return ShowError(log, "Evenemang med id " + id.Value + " är antingen borttaget ur databasen eller felaktigt angivet.", false);
                }

                ViewBag.ev = evenemang.Namn;

                // Förseningsavgift
                var f = smuService.HamtaForseningsavfigt(evenemang.Id);

                SUPVM = new SignMeUpVM
                {
                    EvenemangsId = evenemang.Id,
                    EvenemangsNamn = evenemang.Namn,
                    KanBetalaMedFaktura = evenemang.Fakturabetalning.HasValue ? evenemang.Fakturabetalning.Value : false,
                    FAVM = f,
                    Steps = smuService.HamtaWizardSteps(evenemang.Id)
                };
            }

            HanteraPaymentError();

            if (id == null && SUPVM != null && SUPVM.EvenemangsId != 0)
            {
                id = SUPVM.EvenemangsId;
            }

            Session["VM"] = SUPVM;

            return View(SUPVM.CurrentStep);
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection form, string prev, string ok)
        {
            var SUPVM = (SignMeUpVM)Session["VM"];

            if (SUPVM == null)
                return ShowError(log, "Ett oväntat fel inträffade, var god försök igen.", true, new Exception("Ingen wizard i Session"));

            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Användare är i steg " + SUPVM.CurrentStepIndex + " av anmälan till " + SUPVM.EvenemangsNamn);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ok))
                {
                    var stegNamn = form.Get("Namn");
                    var steg = SUPVM.Steps.FirstOrDefault(s => s.Namn == stegNamn);

                    if (steg == null)
                        return ShowError(log, "Ett oväntat fel inträffade, var god försök igen.", true, new Exception("Wizard steg finns inte i TempData"));

                    // Kopiera värdena till listan av fält
                    foreach (var falt in steg.FaltLista)
                    {
                        falt.Varde = form.Get(falt.Namn);
                    }

                    // Om det är sista steget vi fyllt i
                    if (SUPVM.CurrentStepIndex + 1 == SUPVM.CountSteps)
                    {
                        return RedirectToAction("BekraftaRegistrering");
                    }

                    // Annars går vi till nästa
                    SUPVM.CurrentStepIndex++;
                }
                else if (!string.IsNullOrEmpty(prev))
                {
                    SUPVM.CurrentStepIndex--;
                }
                else
                {
                    return ShowError(log, "Ett oväntat fel inträffade.", false);
                }
            }
            else if (!string.IsNullOrEmpty(prev))
            {
                // Even if validation failed we allow the user to
                // navigate to previous steps
                SUPVM.CurrentStepIndex--;
            }

            // Om deltagarsteget, hämta antal deltagare och
            // krav på personnummer från steget innan
            if (SUPVM.Steps[SUPVM.CurrentStepIndex].Namn == "Deltagare")
            {
                var registrationStep = SUPVM.Steps.FirstOrDefault(stepps => stepps.Namn == "Registrering");
                if (registrationStep != null)
                {
                    var valBana = registrationStep.FaltLista.FirstOrDefault(val => val.Namn == "Bana");
                    var banId = int.Parse(valBana.Varde);
                    var bana = smuService.Db.Banor.FirstOrDefault(b => b.Id == banId);
                    // Om listan av deltagarfält är tom eller om antalet deltagare inte stämmer skapar vi en ny lista
                    if (SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista == null ||
                        SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista.Count != (bana.AntalDeltagare * 2))
                    {
                        var lista = new List<FaltViewModel>();
                        for (int i = 0; i < bana.AntalDeltagare; i++)
                        {
                            lista.Add(new FaltViewModel { Namn = "Förnamn " + (i + 1), Kravs = true });
                            lista.Add(new FaltViewModel { Namn = "Efternamn " + (i + 1), Kravs = true });
                        }
                        SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista = lista;
                    }
                }
            }

            Session["VM"] = SUPVM;

            return View(SUPVM.CurrentStep);
        }

        /// <summary>
        /// Berkäfta registrering Get
        /// </summary>
        /// <returns></returns>
        public ActionResult BekraftaRegistrering()
        {
            var SUPVM = (SignMeUpVM)Session["VM"];

            if (SUPVM == null)
            {
                return ShowError(log, "Ett oväntat fel uppstod. Var god försök senare.", true, new Exception("View model var null i Session. (BekraftaRegistrering POST)"));
            }

            ViewBag.ev = SUPVM.EvenemangsNamn;
            
            LogDebug(log, "Användare bekräfta registrering (GET) för " + SUPVM.EvenemangsNamn);

            Session["VM"] = SUPVM;
            return View(SUPVM);
        }

        /// <summary>
        /// Bekräfta registrering
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BekraftaRegistrering(FormCollection form)
        {
            var SUPVM = (SignMeUpVM)Session["VM"];

            if (SUPVM == null)
            {
                return ShowError(log, "Ett oväntat fel uppstod. Var god försök senare.", true, new Exception("Vymodellen är null i Session. (BekraftaRegistrering POST)"));
            }

            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Användare bekräfta registrering (POST) för " + SUPVM.EvenemangsNamn);

            Session["VM"] = SUPVM;

            // Korrigera
            if (!string.IsNullOrEmpty(Request["korrigera"]))
            {
                return RedirectToAction("Index");
            }
            // Payson
            else if (!string.IsNullOrEmpty(Request["betala"]))
            {
#if DEBUG
                // Spara i databasen
                var reg = smuService.Spara(SUPVM);
                Session["VM"] = null;
                return RedirectToAction("BekraftelseBetalning", new { id = reg.Id });
#else
                return RedirectToAction("Index", "Payson");
#endif
            }
            // Faktura
            else if (!string.IsNullOrEmpty(Request["faktura"]))
            {
                return RedirectToAction("Fakturaadress");
            }
            // Rabatt
            else if (!string.IsNullOrEmpty(Request["rabatt"]))
            {
                var rabattkod = form.Get("Rabattkod");
                if (!string.IsNullOrEmpty(rabattkod))
                {
                    var rabatt = smuService.Db.Rabatter.FirstOrDefault(r => r.Kod == rabattkod && r.EvenemangsId == SUPVM.EvenemangsId);
                    if (rabatt != null)
                    {
                        SUPVM.Rabatt = new RabattVM
                        {
                            Id = rabatt.Id,
                            Kod = rabatt.Kod,
                            Summa = rabatt.Summa
                        };
                    }
                    else
                    {
                        ViewBag.RabattError = "Rabattkoden är inte giltig eller felaktig";
                    }
                }
                else
                {
                    ViewBag.RabattError = "Ingen rabattkod angiven";
                }
            }

            Session["VM"] = SUPVM;
            return View(SUPVM);
        }

        /// <summary>
        /// Faktura Get
        /// </summary>
        /// <returns></returns>
        public ActionResult Fakturaadress()
        {
            var SUPVM = (SignMeUpVM)Session["VM"];

            if (SUPVM == null)
            {
                return RedirectToAction("Index");
            }

            if (SUPVM.Fakturaadress == null)
                SUPVM.Fakturaadress = new InvoiceViewModel();

            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Användare valt faktura (GET) för " + SUPVM.EvenemangsNamn);

            Session["VM"] = SUPVM;
            return View(SUPVM.Fakturaadress);
        }

        /// <summary>
        /// Faktura
        /// </summary>
        /// <param name="fakturaadress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Fakturaadress(InvoiceViewModel fakturaadress)
        {
            var SUPVM = (SignMeUpVM)Session["VM"];

            if (SUPVM == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Användare valt faktura (POST) för " + SUPVM.EvenemangsNamn);

            if (!string.IsNullOrEmpty(Request["cancel"]))
            {
                // Användaren tryckte på korrigera
                Session["VM"] = SUPVM;
                return RedirectToAction("BekraftaRegistrering");
            }

            if (ModelState.IsValid)
            {
                SUPVM.Fakturaadress = fakturaadress;
                // Spara i databasen
                var reg = smuService.Spara(SUPVM);
                
                SkickaRegMail(reg);
                SkickaFaktura(reg);

                Session["VM"] = null;

                return RedirectToAction("BekraftelseBetalning", new { id = reg.Id });
            }
            else
            {
                // Något är fel, vi visar formuläret igen
                Session["VM"] = SUPVM;
                return View(fakturaadress);
            }
        }

        /// <summary>
        /// Visa en registrering efter genomförd betalning
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public ActionResult BekraftelseBetalning(int? id)
        {
            if (id == null)
                return ShowError(log, "Fel vid hämtning av registreringsinformation. Kontrollera i startlistan om er registrering genomförts.", false);

            var reg = smuService.GetRegistrering(id.Value, true);

            ViewBag.ev = reg.Evenemang.Namn;

            LogDebug(log, "Användare bekräftelse betalning (GET) för " + reg.Evenemang.Namn);

            return View(smuService.FillRegistrering(reg));
        }

        /// <summary>
        /// Skicka mail till den som registrerat sig igen
        /// </summary>
        /// <param name="id">Id på registrering</param>
        /// <returns></returns>
        public ActionResult SkickaMailIgen(int? id)
        {
            if (id == null)
            {
                return ShowError(log, "Ingen anmälan med id " + id.Value + " hittades.", false);
            }

            var reg = smuService.Db.Registreringar.Include("Evenemang.Organisation").SingleOrDefault(r => r.Id == id);

            LogDebug(log, "Skicka mail igen för " + reg.Evenemang.Namn + " lagnamn: " + reg.Lagnamn);

            SkickaRegMail(reg);

            return View();
        }

        private void HanteraPaymentError()
        {
            if (TempData["PaymentErrorMessage"] != null)
            {
                ViewBag.PaymentErrorMessage = TempData["PaymentErrorMessage"];
                ViewBag.PaymentErrorParameter = TempData["PaymentErrorParameter"];
                TempData["PaymentErrorMessage"] = null;
                TempData["PaymentErrorParameter"] = null;
            }
        }
    }
}
