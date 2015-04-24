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

            var wizard = new WizardViewModel();
            wizard.Initialize();
            wizard.Evenemang_Id = e.Value;
            TempData["wizard"] = wizard;

            return View(wizard);
        }

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
                    // TODO: we have finished: all the step partial
                    // view models have passed validation => map them
                    // back to the domain model and do some processing with
                    // the results

                    return Content("thanks for filling this form", "text/plain");
                }
            }
            else if (!string.IsNullOrEmpty(Request["prev"]))
            {
                // Even if validation failed we allow the user to
                // navigate to previous steps
                wizard.CurrentStepIndex--;
            }

            // Om registreringssteget, populera drop down listor
            if (wizard.Steps[wizard.CurrentStepIndex] is RegistrationStep)
            {
                ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn");
                ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn");
                ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn");
            }

            // Om deltagarsteget, hämta antal deltagare och
            // krav på personnummer från steget innan
            if (wizard.Steps[wizard.CurrentStepIndex] is DeltagareStep)
            {
                var registrationStep = (RegistrationStep)wizard.Steps.FirstOrDefault<IWizardStep>(stepps => stepps is RegistrationStep);
                if (registrationStep != null)
                {
                    var deltagarStep = (DeltagareStep)wizard.Steps[wizard.CurrentStepIndex];
                    deltagarStep.KravPersonnummer = registrationStep.Ranking;
                    var bana = db.Banor.FirstOrDefault(b => b.ID == registrationStep.Bana);
                    deltagarStep.AntalDeltagareBana = bana.AntalDeltagare;
                }
            }

            TempData["wizard"] = wizard;
            return View(wizard);
        }

        public ActionResult ListaEvenemang()
        {   
            var evenemangsLista = from eve in db.Evenemang
                                    where eve.RegStop >= DateTime.Now
                                    orderby eve.RegStart descending
                                        select eve;

            var listan = evenemangsLista.ToList();
            return View("ListaEvenemang", listan);
        }

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

        //// GET: SignMeUp
        //public ActionResult Index() // TODO evenemang query
        //{
        //    var registreringar = db.Registreringar.Include(r => r.Banor).Include(r => r.Evenemang).Include(r => r.Kanoter).Include(r => r.Klasser);
        //    return View(registreringar.ToList());
        //}

        //// GET: SignMeUp/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Registreringar registreringar = db.Registreringar.Find(id);
        //    if (registreringar == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(registreringar);
        //}

        //// GET: SignMeUp/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn");
        //    ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn");
        //    ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn");
        //    ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn");
        //    return View();
        //}

        //// POST: SignMeUp/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,Adress,Telefon,Epost,Ranking,Startnummer,Lagnamn,Kanot,Klubb,Klass,HarBetalt,Forseningsavgift,Registreringstid,Kommentar,Bana,Rabatter,PaysonToken,Evenemang_Id,Invoice")] Registreringar registreringar)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Registreringar.Add(registreringar);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn", registreringar.Bana);
        //    ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn", registreringar.Evenemang_Id);
        //    ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn", registreringar.Kanot);
        //    ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn", registreringar.Klass);
        //    return View(registreringar);
        //}

        //// GET: SignMeUp/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Registreringar registreringar = db.Registreringar.Find(id);
        //    if (registreringar == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn", registreringar.Bana);
        //    ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn", registreringar.Evenemang_Id);
        //    ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn", registreringar.Kanot);
        //    ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn", registreringar.Klass);
        //    return View(registreringar);
        //}

        //// POST: SignMeUp/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,Adress,Telefon,Epost,Ranking,Startnummer,Lagnamn,Kanot,Klubb,Klass,HarBetalt,Forseningsavgift,Registreringstid,Kommentar,Bana,Rabatter,PaysonToken,Evenemang_Id,Invoice")] Registreringar registreringar)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(registreringar).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn", registreringar.Bana);
        //    ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn", registreringar.Evenemang_Id);
        //    ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn", registreringar.Kanot);
        //    ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn", registreringar.Klass);
        //    return View(registreringar);
        //}

        //// GET: SignMeUp/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Registreringar registreringar = db.Registreringar.Find(id);
        //    if (registreringar == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(registreringar);
        //}

        //// POST: SignMeUp/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Registreringar registreringar = db.Registreringar.Find(id);
        //    db.Registreringar.Remove(registreringar);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
