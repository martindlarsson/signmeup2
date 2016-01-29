using System;
using System.Linq;
using System.Web.Mvc;
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

            // Om view model innehåller ett formulärsid som inte stämmer med inkommande id, nolla wizard
            if (SUPVM != null && id != null && SUPVM.FormularsId != id.Value)
            {
                SUPVM = null;
            }

            if (SUPVM == null)
            {
                if (id == null)
                    return ShowError(log, "Inget formulär angivit. Klicka på länken nedan och välj ett formulär.", false);

                var formular = smuService.GetFormular(id.Value);

                LogDebug(log, "Användare går in på anmälan för: " + formular.Namn);

                var evenemangResult = EvenemangHelper.EvaluateEvenemang(formular.Evenemang);

                if (evenemangResult == EvenemangHelper.EvenemangValidationResult.NotOpen)
                {
                    ViewBag.Closed = false;
                    ViewBag.NotOpen = true;
                    return View("RegNotOpen", formular);
                }
                else if (evenemangResult == EvenemangHelper.EvenemangValidationResult.Closed)
                {
                    ViewBag.Closed = true;
                    ViewBag.NotOpen = false;
                    return View("RegNotOpen", formular);
                }
                else if (evenemangResult == EvenemangHelper.EvenemangValidationResult.DoesNotExist)
                {
                    return ShowError(log, "Evenemang med id " + id.Value + " är antingen borttaget ur databasen eller felaktigt angivet.", false);
                }

                ViewBag.ev = formular.Evenemang.Namn;

                // Förseningsavgift
                var f = smuService.HamtaForseningsavfigt(formular.EvenemangsId.Value);

                SUPVM = new SignMeUpVM
                {
                    EvenemangsId = formular.EvenemangsId.Value,
                    FormularsId = formular.Id,
                    EvenemangsNamn = formular.Namn,
                    KanBetalaMedFaktura = formular.Evenemang.Fakturabetalning.HasValue ? formular.Evenemang.Fakturabetalning.Value : false,
                    FAVM = f,
                    Formular = formular
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
                    var stegId = form.Get("StegId");
                    var steg = SUPVM.Steps.FirstOrDefault(s => s.Id.ToString() == stegId);

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
            //if (SUPVM.Steps[SUPVM.CurrentStepIndex].Namn == "Deltagare")
            //{
            //    var registrationStep = SUPVM.Steps.FirstOrDefault(stepps => stepps.Namn == "Registrering");
            //    if (registrationStep != null)
            //    {
            //        var valBana = registrationStep.FaltLista.FirstOrDefault(val => val.Namn == "Bana");
            //        var banId = int.Parse(valBana.Varde);
            //        var bana = smuService.Db.Banor.FirstOrDefault(b => b.Id == banId);
            //        // Om listan av deltagarfält är tom eller om antalet deltagare inte stämmer skapar vi en ny lista
            //        if (SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista == null ||
            //            SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista.Count != (bana.AntalDeltagare * 2))
            //        {
            //            var lista = new List<FaltViewModel>();
            //            for (int i = 0; i < bana.AntalDeltagare; i++)
            //            {
            //                lista.Add(new FaltViewModel { Namn = "Förnamn " + (i + 1), Kravs = true });
            //                lista.Add(new FaltViewModel { Namn = "Efternamn " + (i + 1), Kravs = true });
            //            }
            //            SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista = lista;
            //        }
            //    }
            //}

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
            
            SetViewBag(SUPVM.EvenemangsId);
            
            LogDebug(log, "Användare bekräfta registrering (GET) för " + SUPVM.EvenemangsNamn);

            Session["VM"] = SUPVM;

            var reg = ClassMapper.MappaTillRegistrering(SUPVM);
            ViewBag.Registrering = reg;

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
                // Om anmälan är gratis, spara och visa bekräftelse
                if (SUPVM.AttBetala == 0)
                {
                    var reg = smuService.Spara(SUPVM);
                    Session["VM"] = null;
                    reg.HarBetalt = true;
                    smuService.UpdateraRegistrering(reg);
                    SkickaRegMail(reg);
                    return RedirectToAction("BekraftelseBetalning", new { id = reg.Id });
                }

                return RedirectToAction("Index", "Payson");
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

            ViewBag.ev = reg.Formular.Evenemang.Namn;
            ViewBag.org = reg.Formular.Evenemang.Organisation.Namn;

            LogDebug(log, "Användare bekräftelse betalning (GET) för " + reg.Formular.Evenemang.Namn);

            return View(reg);
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

            var reg = smuService.GetRegistrering(id.Value, true); // Db.Registreringar.Include("Evenemang.Organisation").SingleOrDefault(r => r.Id == id);

            LogDebug(log, "Skicka mail igen för " + reg.Formular.Evenemang.Namn + " regid: " + reg.Id);

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
