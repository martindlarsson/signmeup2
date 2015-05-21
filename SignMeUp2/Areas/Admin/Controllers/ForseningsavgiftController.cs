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
    public class ForseningsavgiftController : AdminBaseController
    {

        // GET: Admin/Forseningsavgift
        public ActionResult Index()
        {
            // TODO visa enbart för valt evenemang
            //var banor = db.Forseningsavgift.ToList();
            return View(db.Forseningsavgift.ToList());
        }

        // GET: Admin/Forseningsavgift/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forseningsavgift forseningsavgift = db.Forseningsavgift.Find(id);
            if (forseningsavgift == null)
            {
                return HttpNotFound();
            }
            return View(forseningsavgift);
        }

        // GET: Admin/Forseningsavgift/Create
        public ActionResult Create()
        {
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn");
            return View();
        }

        // POST: Admin/Forseningsavgift/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Namn,FranDatum,TillDatum,PlusEllerMinus,Summa,EvenemangsId")] Forseningsavgift forseningsavgift)
        {
            if (ModelState.IsValid)
            {
                db.Forseningsavgift.Add(forseningsavgift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", forseningsavgift.EvenemangsId);
            return View(forseningsavgift);
        }

        // GET: Admin/Forseningsavgift/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forseningsavgift forseningsavgift = db.Forseningsavgift.Find(id);
            if (forseningsavgift == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", forseningsavgift.EvenemangsId);
            return View(forseningsavgift);
        }

        // POST: Admin/Forseningsavgift/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Namn,FranDatum,TillDatum,PlusEllerMinus,Summa,EvenemangsId")] Forseningsavgift forseningsavgift)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forseningsavgift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", forseningsavgift.EvenemangsId);
            return View(forseningsavgift);
        }

        // GET: Admin/Forseningsavgift/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forseningsavgift forseningsavgift = db.Forseningsavgift.Find(id);
            if (forseningsavgift == null)
            {
                return HttpNotFound();
            }
            return View(forseningsavgift);
        }

        // POST: Admin/Forseningsavgift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Forseningsavgift forseningsavgift = db.Forseningsavgift.Find(id);
            db.Forseningsavgift.Remove(forseningsavgift);
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
