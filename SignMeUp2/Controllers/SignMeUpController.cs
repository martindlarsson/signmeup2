using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.DataModel;
using SignMeUp2.Models;
using SignMeUp2.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Controllers
{
    public class SignMeUpController : Controller
    {
        private SignMeUpDataModel db = new SignMeUpDataModel();

        public ActionResult Index(int? e)
        {
            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];

            if (wizard == null)
            {
                if (e == null)
                    return ShowError("Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.");

                var evenemang = db.Evenemang.Where(ev => ev.Id == e).FirstOrDefault();

                var evenemangResult = EvenemangHelper.EvaluateEvenemang(evenemang);

                if (evenemangResult == EvenemangHelper.EvenemangValidationResult.NotOpen)
                {
                    return View("RegNotOpen");
                }
                else if (evenemangResult == EvenemangHelper.EvenemangValidationResult.Closed)
                {
                    return View("RegClosed");
                }
                else if (evenemangResult == EvenemangHelper.EvenemangValidationResult.DoesNotExist)
                {
                    return ShowError("Evenemang med id " + e.Value + " är antingen borttaget ur databasen eller felaktigt angivet.");
                }

                ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn");
                ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn");
                ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn");

                wizard = new WizardViewModel();
                wizard.Initialize();
                wizard.Evenemang_Id = e.Value;
            }

            TempData["wizard"] = wizard;

            return View(wizard);
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(IWizardStep step)
        {
            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];

            if (wizard == null)
                return ShowError("Ett oväntat fel inträffade, var god försök igen.");

            wizard.UpdateSetp(step);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Request["next"]))
                {
                    wizard.CurrentStepIndex++;
                }
                else if (!string.IsNullOrEmpty(Request["prev"]))
                {
                    wizard.CurrentStepIndex--;
                }
                else
                {
                    TempData["wizard"] = wizard;

                    return RedirectToAction("BekraftaRegistrering");
                }
            }
            else if (!string.IsNullOrEmpty(Request["prev"]))
            {
                // Even if validation failed we allow the user to
                // navigate to previous steps
                wizard.CurrentStepIndex--;
            }

            // Om registreringssteget, populera drop down listor
            if (wizard.Steps[wizard.CurrentStepIndex] is RegistrationViewModel)
            {
                ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn");
                ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn");
                ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn");
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
                    var bana = db.Banor.FirstOrDefault(b => b.ID == registrationStep.Bana);
                    deltagarStep.AntalDeltagareBana = bana.AntalDeltagare;
                }
            }

            TempData["wizard"] = wizard;
            return View(wizard);
        }

        /// <summary>
        /// Berkäfta registrering Get
        /// </summary>
        /// <returns></returns>
        public ActionResult BekraftaRegistrering()
        {
            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];

            var regStep = wizard.Steps.OfType<RegistrationViewModel>().FirstOrDefault();

            regStep.Banor = db.Banor.Find(regStep.Bana);
            regStep.Kanoter = db.Kanoter.Find(regStep.Kanot);
            regStep.Klasser = db.Klasser.Find(regStep.Klass);

            TempData["wizard"] = wizard;
            return View(wizard);
        }

        /// <summary>
        /// Bekräfta registrering
        /// </summary>
        /// <param name="wizard"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BekraftaRegistrering(WizardViewModel wizard)
        {
            var tempWizard = (WizardViewModel)TempData["wizard"];

            if (!string.IsNullOrEmpty(Request["korrigera"]))
            {
                TempData["wizard"] = tempWizard;
                return RedirectToAction("Index");
            }
            else if (!string.IsNullOrEmpty(Request["betala"]))
            {
                // Payson
                TempData["wizard"] = tempWizard;
                return View(tempWizard);
            }
            else if (!string.IsNullOrEmpty(Request["faktura"]))
            {
                TempData["wizard"] = tempWizard;
                return RedirectToAction("Faktura");
            }

            return View(tempWizard);
        }

        /// <summary>
        /// Faktura Get
        /// </summary>
        /// <returns></returns>
        public ActionResult Faktura()
        {
            var tempWizard = (WizardViewModel)TempData["wizard"];
            if (tempWizard.Fakturaadress == null)
                tempWizard.Fakturaadress = new Invoice();

            TempData["wizard"] = tempWizard;
            return View(tempWizard.Fakturaadress);
        }

        /// <summary>
        /// Faktura
        /// </summary>
        /// <param name="fakturaadress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Faktura(Invoice fakturaadress)
        {
            var tempWizard = (WizardViewModel)TempData["wizard"];

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
                var reg = SparaNyRegistrering(tempWizard, false);
                TempData["wizard"] = null;
                TempData["reg"] = reg;
                return RedirectToAction("BekraftaBetalning");
            }
            else
            {
                // Något är fel, vi visar formuläret igen
                TempData["wizard"] = tempWizard;
                return View(fakturaadress);
            }
        }

        /// <summary>
        /// Lista alla evenemang
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaEvenemang()
        {   
            var evenemangsLista = from eve in db.Evenemang
                                    where eve.RegStop >= DateTime.Now
                                    orderby eve.RegStart descending
                                        select eve;

            var listan = evenemangsLista.ToList();
            return View("ListaEvenemang", listan);
        }

        /// <summary>
        /// Visa en registrering efter genomförd betalning
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public ActionResult BekraftaBetalning()
        {
            var reg = (Registreringar)TempData["reg"];
            if (reg == null)
                return ShowError("Fel vid hämtning av registreringsinformation. Kontrollera i startlistan om er registrering genomförts.");
            
            return View(PopuleraRegistrering(reg));
        }

        /// <summary>
        /// Visa startlista
        /// </summary>
        /// <param name="e">Evenemangs ID</param>
        /// <returns></returns>
        public ActionResult Startlista(int? e)
        {
            if (e == null)
                return ShowError("Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.");

            var evenemang = db.Evenemang.Where(ev => ev.Id == e).FirstOrDefault();

            var evenemangResult = EvenemangHelper.EvaluateEvenemang(evenemang);

            if (evenemangResult == EvenemangHelper.EvenemangValidationResult.DoesNotExist)
            {
                return ShowError("Evenemang med id " + e.Value + " är antingen borttaget ur databasen eller felaktigt angivet.");
            }

            var regs = db.Registreringar.Where(reg => reg.Evenemang_Id == e.Value).ToList();
            var banor = db.Banor.ToList();
            var klasser = db.Klasser.ToList();

            var startlista = StartlistaViewModel.GetStartlist(regs, evenemang.Namn, banor, klasser);

            return View("Startlista", startlista);
        }

        /// <summary>
        /// ShowError
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected ActionResult ShowError(string logMessage, Exception exception = null)
        {
            //log.Error(logMessage, exception);
            //try
            //{
            //    if (exception != null)
            //    {
            //        SendMail.SendErrorMessage(logMessage + "\n\n" + exception.Message + "\n\n" + exception.StackTrace);
            //    }
            //    else
            //    {
            //        SendMail.SendErrorMessage(logMessage);
            //    }
            //}
            //catch (Exception exc)
            //{
            //    log.Error("Error sending error mail.", exc);
            //}

            //TempData["Error"] = "Fel vid anmälna. Administratör är kontaktad. Vill du veta när felet blivit åtgärdat skicka gärna ett meddelande till utmaningen@karlstadmultisport.se";

            TempData["error"] = logMessage;

            return View("Error");
        }

        /// <summary>
        /// Spara en ny registrering i databasen
        /// </summary>
        /// <param name="wizard"></param>
        private Registreringar SparaNyRegistrering(WizardViewModel wizard, bool harBetalt)
        {
            var reg = Helpers.ClassMapper.MapToRegistreringar(wizard);
            reg.Registreringstid = DateTime.Now;
            reg.HarBetalt = harBetalt;
            db.Registreringar.Add(reg);
            foreach (var deltagare in reg.Deltagare)
            {
                db.Deltagare.Add(deltagare);
            }

            if (reg.Invoices != null)
                db.Invoice.Add(reg.Invoices);

            db.SaveChanges();

            db.Entry(reg).GetDatabaseValues();
            return reg;
        }

        /// <summary>
        /// Populera registreringen med Bana, Klass mm
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private Registreringar PopuleraRegistrering(Registreringar reg)
        {
            reg.Evenemang = db.Evenemang.Find(reg.Evenemang_Id);
            reg.Banor = db.Banor.Find(reg.Bana);
            reg.Klasser = db.Klasser.Find(reg.Klass);
            reg.Kanoter = db.Kanoter.Find(reg.Kanot);

            // Deltagare ??

            // Invoice ??

            return reg;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
