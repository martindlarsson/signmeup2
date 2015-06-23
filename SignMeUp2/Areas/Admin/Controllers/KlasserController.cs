using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class KlasserController : AdminBaseController
    {
        private static string _entity = "Klasser";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Klasser
        public ActionResult Index(int? id)
        {
            IQueryable<Klasser> klasser;

            if (id != null)
            {
                klasser = db.Klasser.Where(k => k.EvenemangsId == id.Value);
                ViewBag.Evenemang = db.Evenemang.FirstOrDefault(e => e.Id == id.Value);
            }
            else if (IsUserAdmin)
            {
                klasser = db.Klasser.Include(k => k.Evenemang);
            }
            else
            {
                return new HttpNotFoundResult();
            }

            return View(klasser.ToList());
        }

        // GET: Klasser/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Klasser klasser = db.Klasser.Find(id);
            if (klasser == null)
            {
                return HttpNotFound();
            }
            return View(klasser);
        }

        // GET: Klasser/Create
        public ActionResult Create()
        {
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn");
            return View();
        }

        // POST: Klasser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Namn,EvenemangsId")] Klasser klasser)
        {
            if (ModelState.IsValid)
            {
                db.Klasser.Add(klasser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", klasser.EvenemangsId);
            return View(klasser);
        }

        // GET: Klasser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Klasser klasser = db.Klasser.Find(id);
            if (klasser == null)
            {
                return HttpNotFound();
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", klasser.EvenemangsId);
            return View(klasser);
        }

        // POST: Klasser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Namn,EvenemangsId")] Klasser klasser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(klasser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", klasser.EvenemangsId);
            return View(klasser);
        }

        // GET: Klasser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Klasser klasser = db.Klasser.Find(id);
            if (klasser == null)
            {
                return HttpNotFound();
            }
            return View(klasser);
        }

        // POST: Klasser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Klasser klasser = db.Klasser.Find(id);
            db.Klasser.Remove(klasser);
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
