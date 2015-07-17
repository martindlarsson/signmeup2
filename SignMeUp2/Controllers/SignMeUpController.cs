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
    public class SignMeUpController : RegBaseController
    {
        public ActionResult Index(int? id)
        {
            SignMeUpVM SUPVM = (SignMeUpVM)TempData["VM"];

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
                    FAVM = f,
                    Steps = smuService.HamtaWizardSteps(evenemang.Id)
                };
            }

            HanteraPaymentError();

            if (id == null && SUPVM != null && SUPVM.EvenemangsId != 0)
            {
                id = SUPVM.EvenemangsId;
            }

            //FillViewBag(SUPVM);

            TempData["VM"] = SUPVM;

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
            var SUPVM = (SignMeUpVM)TempData["VM"];

            if (SUPVM == null)
                return ShowError(log, "Ett oväntat fel inträffade, var god försök igen.", true, new Exception("Ingen wizard i TempData"));

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

            // Om registreringssteget, populera drop down listor
            //if (SUPVM.Steps[SUPVM.CurrentStepIndex] is RegistrationViewModel)
            //{
            //    FillViewBag(SUPVM);
            //}

            // Om vi just fyllt i registreringssteget, fyll på bana, klass och kanot
            // OBS, förutsätter att RegStep är på position 0
            //if (SUPVM.Wizard.CurrentStepIndex > 0 && SUPVM.Wizard.Steps[SUPVM.Wizard.CurrentStepIndex - 1] is RegistrationViewModel)
            //{
            //    SUPVM.Wizard = FillRegStep(SUPVM.Wizard);
            //}

            // Om deltagarsteget, hämta antal deltagare och
            // krav på personnummer från steget innan
            if (SUPVM.Steps[SUPVM.CurrentStepIndex].Namn == "Deltagare")
            {
                var registrationStep = SUPVM.Steps.FirstOrDefault(stepps => stepps.Namn == "Registrering");
                if (registrationStep != null)
                {
                    var valBana = registrationStep.FaltLista.FirstOrDefault(val => val.Namn == "Bana");
                    var banaId = int.Parse(valBana.Varde);
                    var bana = smuService.Db.Banor.FirstOrDefault(b => b.Id == banaId);
                    // Om listan av deltagarfält är tom eller om antalet deltagare inte stämmer skapar vi en ny lista
                    if (SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista == null ||
                        SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista.Count != bana.AntalDeltagare)
                    {
                        var lista = new List<FaltViewModel>();
                        for (int i = 0; i < bana.AntalDeltagare; i++)
                        {
                            lista.Add(new FaltViewModel { Namn = "Deltagare " + i + 1, Kravs = true });
                        }
                        SUPVM.Steps[SUPVM.CurrentStepIndex].FaltLista = lista;
                    }
                }
            }

            TempData["VM"] = SUPVM;

            return View(SUPVM.CurrentStep);
        }

        //private WizardViewModel FillRegStep(WizardViewModel wizard)
        //{
        //    RegistrationViewModel regStep = wizard.Steps.Where(s => s is RegistrationViewModel).FirstOrDefault() as RegistrationViewModel;

        //    if (regStep == null)
        //        throw new ArgumentException("No RegistrationViewModel found in WizardViewModel.Steps.");

        //    if (regStep.Banor == null)
        //    {
        //        regStep.Banor = smuService.Db.Banor.Find(regStep.Bana);
        //    }
        //    if (regStep.Klasser == null)
        //    {
        //        regStep.Klasser = smuService.Db.Klasser.Find(regStep.Klass);
        //    }
        //    if (regStep.Kanoter == null)
        //    {
        //        regStep.Kanoter = smuService.Db.Kanoter.Find(regStep.Kanot);
        //    }

        //    return wizard;
        //}

        /// <summary>
        /// Berkäfta registrering Get
        /// </summary>
        /// <returns></returns>
        public ActionResult BekraftaRegistrering()
        {
            var SUPVM = (SignMeUpVM)TempData["VM"];

            if (SUPVM == null)
            {
                return ShowError(log, "Ett oväntat fel uppstod. Var god försök senare.", true, new Exception("View model var null i TempData. (BekraftaRegistrering POST)"));
            }

            ViewBag.ev = SUPVM.EvenemangsNamn;

            //var regStep = SUPVM.GetRegStep();

            //var banor = smuService.Db.Banor.Find(regStep.Bana);
            //var kanoter = smuService.Db.Kanoter.Find(regStep.Kanot);
            //var klasser = smuService.Db.Klasser.Find(regStep.Klass);
            //SUPVM.Betalnignsposter = new BetalningViewModel(banor, kanoter, SUPVM.Rabatt, SUPVM.FAVM);
            
            LogDebug(log, "Användare bekräfta registrering (GET) för " + SUPVM.EvenemangsNamn);

            TempData["VM"] = SUPVM;
            return View(SUPVM);
        }

        /// <summary>
        /// Bekräfta registrering
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BekraftaRegistrering(SignMeUpVM tempSUPVM)
        {
            var SUPVM = (SignMeUpVM)TempData["VM"];
            //var tempWizard = (WizardViewModel)TempData["wizard"];

            if (SUPVM == null)
            {
                return ShowError(log, "Ett oväntat fel uppstod. Var god försök senare.", true, new Exception("Vymodellen är null i TempData. (BekraftaRegistrering POST)"));
            }

            ViewBag.ev = SUPVM.EvenemangsNamn;

            //var regStep = SUPVM.GetRegStep();

            LogDebug(log, "Användare bekräfta registrering (POST) för " + SUPVM.EvenemangsNamn);

            TempData["VM"] = SUPVM;

            // Korrigera
            if (!string.IsNullOrEmpty(Request["korrigera"]))
            {
                return RedirectToAction("Index");
            }
            // Payson
            else if (!string.IsNullOrEmpty(Request["betala"]))
            {
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
                if (!string.IsNullOrEmpty(SUPVM.Rabattkod))
                {
                    var rabatt = smuService.Db.Rabatter.FirstOrDefault(r => r.Kod == SUPVM.Rabattkod && r.EvenemangsId == SUPVM.EvenemangsId);
                    if (rabatt != null)
                    {
                        SUPVM.Rabatt = new RabattVM
                        {
                            Id = rabatt.Id,
                            Kod = rabatt.Kod,
                            Summa = rabatt.Summa
                        };
                        SUPVM.Rabattkod = rabatt.Kod;
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

            // TODO bättre lösning på betalningslistan!!
            //var bana = smuService.Db.Banor.FirstOrDefault(b => b.Id == SUPVM.GetRegStep().Bana);
            //var kanot = smuService.Db.Kanoter.FirstOrDefault(k => k.Id == SUPVM.GetRegStep().Kanot);
            //SUPVM.Betalnignsposter = new BetalningViewModel(bana, kanot, SUPVM.Rabatt, SUPVM.FAVM);

            TempData["VM"] = SUPVM;
            return View(SUPVM);
        }

        /// <summary>
        /// Faktura Get
        /// </summary>
        /// <returns></returns>
        public ActionResult Fakturaadress()
        {
            var SUPVM = (SignMeUpVM)TempData["VM"];

            if (SUPVM == null)
            {
                return RedirectToAction("Index");
            }

            if (SUPVM.Fakturaadress == null)
                SUPVM.Fakturaadress = new InvoiceViewModel();

            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Användare valt faktura (GET) för " + SUPVM.EvenemangsNamn);

            TempData["VM"] = SUPVM;
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
            //var tempWizard = (WizardViewModel)TempData["wizard"];
            var SUPVM = (SignMeUpVM)TempData["VM"];

            if (SUPVM == null)
            {
                return RedirectToAction("Index");
            }

            //var ev = smuService.HamtaEvenemang(tempWizard.Evenemang_Id);
            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Användare valt faktura (POST) för " + SUPVM.EvenemangsNamn);

            if (!string.IsNullOrEmpty(Request["cancel"]))
            {
                // Användaren tryckte på korrigera
                TempData["VM"] = SUPVM;
                return RedirectToAction("BekraftaRegistrering");
            }

            if (ModelState.IsValid)
            {
                SUPVM.Fakturaadress = fakturaadress;
                // Spara i databasen
                //FillRegStep(tempWizard);
                //var reg = smuService.SparaNyRegistrering(tempWizard);
                throw new NotImplementedException();
                TempData["VM"] = null;
                // TODO hämta registreringsid!!
                return RedirectToAction("BekraftelseBetalning", new { id = 1 });
            }
            else
            {
                // Något är fel, vi visar formuläret igen
                TempData["VM"] = SUPVM;
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

        //private void FillViewBag(SignMeUpVM SUPVM)
        //{
        //    ViewBag.Bana = new SelectList(smuService.Db.Banor.Where(b => b.EvenemangsId == SUPVM.EvenemangsId).ToList(), "ID", "Namn");
        //    ViewBag.Kanot = new SelectList(smuService.Db.Kanoter.Where(b => b.EvenemangsId == SUPVM.EvenemangsId).ToList(), "ID", "Namn");
        //    ViewBag.Klass = new SelectList(smuService.Db.Klasser.Where(b => b.EvenemangsId == SUPVM.EvenemangsId).ToList(), "ID", "Namn");
        //}

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
