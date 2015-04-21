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
//using MvcContrib.Attributes;

namespace SignMeUp2.Controllers
{
    public class SignMeUpController : Controller
    {
        private SignMeUpDataModel db = new SignMeUpDataModel();

        public ActionResult Index(int? e)
        {
            if (e == null)
                throw new Exception("Inget evenemang har angivits.");
                //return ShowError("Inget evenemang har angivits.");

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

            ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn");
            //ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn");
            ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn");
            ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn");

            var wizard = new WizardViewModel();
            wizard.Initialize();
            //wizard.Registrering = new Registreringar { Evenemang_Id = e.Value };
            wizard.Evenemang_Id = e.Value;
            TempData["wizard"] = wizard;
            //return View(wizard.Steps[0]);
            return View(wizard);
        }

        [HttpPost]
        public ActionResult Index(/*WizardViewModel wizard,*/ IWizardStep step)
        {
            // TODO, om bana valt, sätt antal deltagare
            //wizard.Steps[wizard.CurrentStepIndex] = step;

            WizardViewModel wizard = (WizardViewModel)TempData["wizard"];
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

            TempData["wizard"] = wizard;
            return View(wizard);
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
