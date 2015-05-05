using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.DataModel;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class KanoterController : BaseController
    {
        // GET: Kanoter
        public ActionResult Index()
        {
            var kanoter = db.Kanoter.Include(k => k.Evenemang);
            return View(kanoter.ToList());
        }

        // GET: Kanoter/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanoter kanoter = db.Kanoter.Find(id);
            if (kanoter == null)
            {
                return HttpNotFound();
            }
            return View(kanoter);
        }

        // GET: Kanoter/Create
        public ActionResult Create()
        {
            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn");
            return View();
        }

        // POST: Kanoter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Namn,Avgift,Evenemang_ID")] Kanoter kanoter)
        {
            if (ModelState.IsValid)
            {
                db.Kanoter.Add(kanoter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn", kanoter.Evenemang_ID);
            return View(kanoter);
        }

        // GET: Kanoter/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanoter kanoter = db.Kanoter.Find(id);
            if (kanoter == null)
            {
                return HttpNotFound();
            }
            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn", kanoter.Evenemang_ID);
            return View(kanoter);
        }

        // POST: Kanoter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Namn,Avgift,Evenemang_ID")] Kanoter kanoter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kanoter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn", kanoter.Evenemang_ID);
            return View(kanoter);
        }

        // GET: Kanoter/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanoter kanoter = db.Kanoter.Find(id);
            if (kanoter == null)
            {
                return HttpNotFound();
            }
            return View(kanoter);
        }

        // POST: Kanoter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kanoter kanoter = db.Kanoter.Find(id);
            db.Kanoter.Remove(kanoter);
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
