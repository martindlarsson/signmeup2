using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Models;
using SignMeUp2.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Controllers
{
    public class SignMeUpController : RegBaseController
    {
        public ActionResult Index(int? id)
        {   
            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];

            if (wizard == null)
            {
                if (id == null)
                    return ShowError("Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.", false);

                var evenemang = smuService.Db.Evenemang.Find(id);

                log.Info("Användare går in på anmälan för: " + evenemang.Namn);

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
                    return ShowError("Evenemang med id " + id.Value + " är antingen borttaget ur databasen eller felaktigt angivet.", false);
                }

                ViewBag.ev = evenemang.Namn;

                // Förseningsavgift
                var f = smuService.HamtaForseningsavfigt(evenemang.Id);

                wizard = new WizardViewModel
                {
                    Evenemang_Id = id.Value,
                    Forseningsavgift = f
                };
                wizard.Initialize();
            }

            if (id == null && wizard != null && wizard.Evenemang_Id != 0)
            {
                id = wizard.Evenemang_Id;
            }

            FillViewBag(id.Value);

            TempData["wizard"] = wizard;

            return View(wizard);
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(IWizardStep step, string prev, string ok)
        {
            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];

            if (wizard == null)
                return ShowError("Ett oväntat fel inträffade, var god försök igen.", true, new Exception("Ingen wizard i TempData"));

            var ev = smuService.HamtaEvenemang(wizard.Evenemang_Id);
            ViewBag.ev = ev.Namn;

            log.Info("Användare är i steg " + wizard.CurrentStepIndex + " av anmälan till " + ev.Namn);

            wizard.UpdateSetp(step);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(ok))
                {
                    TempData["wizard"] = wizard;

                    // Om det är sista steget vi fyllt i
                    if (wizard.CurrentStepIndex + 1 == wizard.CountSteps)
                    {
                        return RedirectToAction("BekraftaRegistrering");
                    }

                    // Annars går vi till nästa
                    wizard.CurrentStepIndex++;
                }
                else if (!string.IsNullOrEmpty(prev))
                {
                    wizard.CurrentStepIndex--;
                }
                else
                {
                    return ShowError("Ett oväntat fel inträffade.", false);
                }
            }
            else if (!string.IsNullOrEmpty(prev))
            {
                // Even if validation failed we allow the user to
                // navigate to previous steps
                wizard.CurrentStepIndex--;
            }

            // Om registreringssteget, populera drop down listor
            if (wizard.Steps[wizard.CurrentStepIndex] is RegistrationViewModel)
            {
                FillViewBag(ev.Id);
            }

            // Om vi just fyllt i registreringssteget, fyll på bana, klass och kanot
            // OBS, förutsätter att RegStep är på position 0
            if (wizard.CurrentStepIndex > 0 && wizard.Steps[wizard.CurrentStepIndex - 1] is RegistrationViewModel)
            {
                wizard = FillRegStep(wizard);
            }

            // Om deltagarsteget, hämta antal deltagare och
            // krav på personnummer från steget innan
            if (wizard.Steps[wizard.CurrentStepIndex] is DeltagareListViewModel)
            {
                var registrationStep = (RegistrationViewModel)wizard.Steps.FirstOrDefault<IWizardStep>(stepps => stepps is RegistrationViewModel);
                if (registrationStep != null)
                {
                    var deltagarStep = (DeltagareListViewModel)wizard.Steps[wizard.CurrentStepIndex];
                    deltagarStep.KravPersonnummer = registrationStep.Ranking;
                    var bana = smuService.Db.Banor.FirstOrDefault(b => b.Id == registrationStep.Bana);
                    deltagarStep.AntalDeltagareBana = bana.AntalDeltagare;
                }
            }

            TempData["wizard"] = wizard;
            return View(wizard);
        }

        private WizardViewModel FillRegStep(WizardViewModel wizard)
        {
            RegistrationViewModel regStep = wizard.Steps.Where(s => s is RegistrationViewModel).FirstOrDefault() as RegistrationViewModel;

            if (regStep == null)
                throw new ArgumentException("No RegistrationViewModel found in WizardViewModel.Steps.");

            if (regStep.Banor == null)
            {
                regStep.Banor = smuService.Db.Banor.Find(regStep.Bana);
            }
            if (regStep.Klasser == null)
            {
                regStep.Klasser = smuService.Db.Klasser.Find(regStep.Klass);
            }
            if (regStep.Kanoter == null)
            {
                regStep.Kanoter = smuService.Db.Kanoter.Find(regStep.Kanot);
            }

            return wizard;
        }

        /// <summary>
        /// Berkäfta registrering Get
        /// </summary>
        /// <returns></returns>
        public ActionResult BekraftaRegistrering()
        {
            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];

            if (wizard == null)
            {
                return ShowError("Ett oväntat fel uppstod. Var god försök senare.", true, new Exception("reg was null in TempData. (BekraftaRegistrering POST)"));
            }

            var ev = smuService.HamtaEvenemang(wizard.Evenemang_Id);
            ViewBag.ev = ev.Namn;

            var regStep = wizard.GetRegStep();

            regStep.Banor = smuService.Db.Banor.Find(regStep.Bana);
            regStep.Kanoter = smuService.Db.Kanoter.Find(regStep.Kanot);
            regStep.Klasser = smuService.Db.Klasser.Find(regStep.Klass);
            wizard.Betalnignsposter = new BetalningViewModel(regStep.Banor, regStep.Kanoter, wizard.Rabatt, wizard.Forseningsavgift);
            
            log.Info("Användare bekräfta registrering (GET) för " + ev.Namn);

            TempData["wizard"] = wizard;
            return View(wizard);
        }

        /// <summary>
        /// Bekräfta registrering
        /// </summary>
        /// <param name="wizard"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BekraftaRegistrering(WizardViewModel viewModel)
        {
            var tempWizard = (WizardViewModel)TempData["wizard"];

            if (tempWizard == null)
            {
                return ShowError("Ett oväntat fel uppstod. Var god försök senare.", true, new Exception("tempWizard was null in TempData. (BekraftaRegistrering POST)"));
            }

            var ev = smuService.HamtaEvenemang(tempWizard.Evenemang_Id);
            ViewBag.ev = ev.Namn;

            var regStep = tempWizard.GetRegStep();

            log.Info("Användare bekräfta registrering (POST) för " + ev.Namn);

            // Korrigera
            if (!string.IsNullOrEmpty(Request["korrigera"]))
            {
                TempData["wizard"] = tempWizard;
                return RedirectToAction("Index");
            }
            // Payson
            else if (!string.IsNullOrEmpty(Request["betala"]))
            {
                TempData["wizard"] = tempWizard;
                FillRegStep(tempWizard);
                var registrering = ClassMapper.MapToRegistreringar(tempWizard);
                TempData["reg"] = registrering;

                return RedirectToAction("Index", "Payson");
            }
            // Faktura
            else if (!string.IsNullOrEmpty(Request["faktura"]))
            {
                TempData["wizard"] = tempWizard;
                return RedirectToAction("Fakturaadress");
            }
            // Rabatt
            else if (!string.IsNullOrEmpty(Request["rabatt"]))
            {
                if (!string.IsNullOrEmpty(viewModel.Rabattkod))
                {
                    var rabatt = smuService.Db.Rabatter.FirstOrDefault(r => r.Kod == viewModel.Rabattkod && r.EvenemangsId == tempWizard.Evenemang_Id);
                    if (rabatt != null)
                    {
                        tempWizard.Rabatt = rabatt;
                        tempWizard.Rabattkod = rabatt.Kod;
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

            tempWizard.Betalnignsposter = new BetalningViewModel(regStep.Banor, regStep.Kanoter, tempWizard.Rabatt, tempWizard.Forseningsavgift);
            TempData["wizard"] = tempWizard;
            return View(tempWizard);
        }

        /// <summary>
        /// Faktura Get
        /// </summary>
        /// <returns></returns>
        public ActionResult Fakturaadress()
        {
            var tempWizard = (WizardViewModel)TempData["wizard"];

            if (tempWizard == null)
            {
                return RedirectToAction("Index");
            }

            if (tempWizard.Fakturaadress == null)
                tempWizard.Fakturaadress = new Invoice();

            var ev = smuService.HamtaEvenemang(tempWizard.Evenemang_Id);
            ViewBag.ev = ev.Namn;

            log.Info("Användare valt faktura (GET) för " + ev.Namn);

            TempData["wizard"] = tempWizard;
            return View(tempWizard.Fakturaadress);
        }

        /// <summary>
        /// Faktura
        /// </summary>
        /// <param name="fakturaadress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Fakturaadress(Invoice fakturaadress)
        {
            var tempWizard = (WizardViewModel)TempData["wizard"];

            if (tempWizard == null)
            {
                return RedirectToAction("Index");
            }

            var ev = smuService.HamtaEvenemang(tempWizard.Evenemang_Id);
            ViewBag.ev = ev.Namn;

            log.Info("Användare valt faktura (POST) för " + ev.Namn);

            if (!string.IsNullOrEmpty(Request["cancel"]))
            {
                // Användaren tryckte på korrigera
                TempData["wizard"] = tempWizard;
                return RedirectToAction("BekraftaRegistrering", tempWizard);
            }

            if (ModelState.IsValid)
            {
                tempWizard.Fakturaadress = fakturaadress;
                // Spara i databasen
                FillRegStep(tempWizard);
                var reg = smuService.SparaNyRegistrering(tempWizard);
                TempData["wizard"] = null;
                TempData["reg"] = null;
                return RedirectToAction("BekraftelseBetalning", new { id = reg.Id });
            }
            else
            {
                // Något är fel, vi visar formuläret igen
                TempData["wizard"] = tempWizard;
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
                return ShowError("Fel vid hämtning av registreringsinformation. Kontrollera i startlistan om er registrering genomförts.", false);

            var reg = smuService.GetRegistrering(id.Value, true);

            ViewBag.ev = reg.Evenemang.Namn;

            log.Info("Användare bekräftelse betalning (GET) för " + reg.Evenemang.Namn);

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
                ShowError("Ingen anmälan med id " + id.Value + " hittades.", false);
            }

            var reg = smuService.Db.Registreringar.Include("Evenemang.Organisation").SingleOrDefault(r => r.Id == id);

            log.Info("Skicka mail igen för " + reg.Evenemang.Namn + " lagnamn: " + reg.Lagnamn);

            SkickaRegMail(reg);

            return View();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        smuService.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
