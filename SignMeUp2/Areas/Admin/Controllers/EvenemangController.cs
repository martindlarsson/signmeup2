using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class EvenemangController : BaseController
    {
        private static string _entity = "Evenemang";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Evenemang
        public ActionResult Index()
        {
            return View(HamtaEvenemangForAnv());
        }

        public ActionResult Oversikt(int? id)
        {
            var evenemang = db.Evenemang.Include("Banor").Include("Klasser").Include("Kanoter").Include("Rabatter").Include("Forseningsavgifter").Include("Registreringar").FirstOrDefault(e => e.Id == id.Value);

            if (evenemang == null)
                ShowError(log, "Hittade inte evenemanget", true);

            return View(evenemang);
        }

        // GET: Evenemang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evenemang evenemang = db.Evenemang.Find(id);
            if (evenemang == null)
            {
                return HttpNotFound();
            }
            return View(evenemang);
        }

        // GET: Evenemang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Evenemang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Namn,RegStart,RegStop,Fakturabetalning,FakturaBetaldSenast")] Evenemang evenemang)
        {
            if (ModelState.IsValid)
            {
                evenemang.OrganisationsId = HamtaUser().OrganisationsId;
                db.Evenemang.Add(evenemang);
                db.SaveChanges();
                db.Entry(evenemang).GetDatabaseValues();
                return RedirectToAction("Oversikt", new { id = evenemang.Id });
            }

            ViewBag.FelMeddelande = "Det finns valideringsfel i formuläret. Korrigera och försök igen.";

            return View(evenemang);
        }

        // GET: Evenemang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evenemang evenemang = db.Evenemang.Find(id);
            if (evenemang == null)
            {
                return HttpNotFound();
            }

            SetViewBag(id);

            return View(evenemang);
        }

        // POST: Evenemang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Namn,RegStart,RegStop,Fakturabetalning,FakturaBetaldSenast")] Evenemang evenemang)
        {
            if (ModelState.IsValid)
            {
                var user = HamtaUser();
                evenemang.OrganisationsId = user.OrganisationsId;
                db.Entry(evenemang).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                ViewBag.FelMeddelande = "Det finns valideringsfel i formuläret. Korrigera och försök igen.";
                SetViewBag(evenemang);
                return View(evenemang);
            }

            ViewBag.Meddelande = "Ändringarna har sparats.";

            SetViewBag(evenemang);
            return View(evenemang);
        }

        // GET: Evenemang/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evenemang evenemang = db.Evenemang.Find(id);
            if (evenemang == null)
            {
                return HttpNotFound();
            }
            return View(evenemang);
        }

        // POST: Evenemang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evenemang evenemang = db.Evenemang.Find(id);
            evenemang.Formular.Clear();

            db.SaveChanges();
            db.Evenemang.Remove(evenemang);
            db.SaveChanges();
            return RedirectToAction("Index");
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
