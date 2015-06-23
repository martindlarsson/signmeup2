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
        private static string _entity = "Kanot";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Kanoter
        public ActionResult Index(int id)
        {
            SetViewBag(id);

            var kanoter = db.Kanoter.Where(k => k.EvenemangsId == id);

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

            SetViewBag(kanoter.EvenemangsId);

            return View(kanoter);
        }

        // GET: Kanoter/Create
        public ActionResult Create(int id)
        {
            SetViewBag(id);

            return View();
        }

        // POST: Kanoter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Namn,Avgift")] Kanoter kanot, int id)
        {
            if (ModelState.IsValid)
            {
                kanot.EvenemangsId = id;
                db.Kanoter.Add(kanot);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });
            }

            SetViewBag(kanot.EvenemangsId);
            return View(kanot);
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

            SetViewBag(kanoter.EvenemangsId);

            return View(kanoter);
        }

        // POST: Kanoter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Namn,Avgift,EvenemangsId")] Kanoter kanoter)
        {
            SetViewBag(kanoter.EvenemangsId);

            if (ModelState.IsValid)
            {
                db.Entry(kanoter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = kanoter.EvenemangsId });
            }

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

            SetViewBag(kanoter.EvenemangsId);

            return View(kanoter);
        }

        // POST: Kanoter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kanoter kanoter = db.Kanoter.Find(id);
            var evId = kanoter.EvenemangsId;
            SetViewBag(kanoter.EvenemangsId);
            db.Kanoter.Remove(kanoter);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = evId });
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
