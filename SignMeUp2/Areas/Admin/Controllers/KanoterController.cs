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
    public class KanoterController : AdminBaseController
    {
        private static string _entitet = "Kanoter";

        private void SetEntitet()
        {
            ViewBag.Entitet = _entitet;
        } 

        // GET: Kanoter
        public ActionResult Index(int? id)
        {
            SetEntitet();

            IQueryable<Kanoter> kanoter;

            if (id != null)
            {
                kanoter = db.Kanoter.Where(k => k.EvenemangsId == id.Value);
                ViewBag.Evenemang = db.Evenemang.FirstOrDefault(e => e.Id == id.Value);
            }
            else if (IsUserAdmin)
            {
                kanoter = db.Kanoter.Include(k => k.Evenemang);
            }
            else
            {
                return new HttpNotFoundResult();
            }

            return View(kanoter.ToList());
        }

        // GET: Kanoter/Details/5
        public ActionResult Details(int? id)
        {
            SetEntitet();

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
        public ActionResult Create(int? id)
        {
            SetEntitet();

            if (id != null)
            {
                ViewBag.Evenemang = db.Evenemang.FirstOrDefault(e => e.Id == id.Value);
                var kanot = new Kanoter { EvenemangsId = id.Value };
                return View(kanot);
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn");

            return View();
        }

        // POST: Kanoter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Namn,Avgift,EvenemangsId")] Kanoter kanoter)
        {
            SetEntitet();

            if (ModelState.IsValid)
            {
                db.Kanoter.Add(kanoter);
                db.SaveChanges();
                return RedirectToAction("Index", kanoter.EvenemangsId);
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", kanoter.EvenemangsId);
            return View(kanoter);
        }

        // GET: Kanoter/Edit/5
        public ActionResult Edit(int? id)
        {
            SetEntitet();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanoter kanoter = db.Kanoter.Find(id);
            if (kanoter == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", kanoter.EvenemangsId);
            return View(kanoter);
        }

        // POST: Kanoter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Namn,Avgift,EvenemangsId")] Kanoter kanoter)
        {
            SetEntitet();

            if (ModelState.IsValid)
            {
                db.Entry(kanoter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", kanoter.EvenemangsId);
            return View(kanoter);
        }

        // GET: Kanoter/Delete/5
        public ActionResult Delete(int? id)
        {
            SetEntitet();

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
            SetEntitet();

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
