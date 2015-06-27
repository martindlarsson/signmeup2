using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SignMeUp2.Data;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class ForseningsavgiftController : AdminBaseController
    {
        private static string _entity = "Förseningsavgift";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Admin/Forseningsavgift
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SetViewBag(id);

            var forseningsavgifter = db.Forseningsavgift.Include(f => f.Evenemang).Where(f => f.EvenemangsId == id.Value);

            return View(forseningsavgifter.ToList());
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
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.PlusEllerMinus = EnumHelper.GetSelectList(typeof(SignMeUp2.Data.TypAvgift));
            SetViewBag(id);

            return View();
        }

        // POST: Admin/Forseningsavgift/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Namn,FranDatum,TillDatum,PlusEllerMinus,Summa")] Forseningsavgift forseningsavgift, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                forseningsavgift.EvenemangsId = id;
                db.Forseningsavgift.Add(forseningsavgift);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });
            }

            SetViewBag(id);
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

            SetViewBag(id);

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
                return RedirectToAction("Index", new { id = forseningsavgift.EvenemangsId });
            }

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

            SetViewBag(forseningsavgift.EvenemangsId);

            return View(forseningsavgift);
        }

        // POST: Admin/Forseningsavgift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Forseningsavgift forseningsavgift = db.Forseningsavgift.Find(id);
            var evId = forseningsavgift.EvenemangsId;

            try
            {
                db.Forseningsavgift.Remove(forseningsavgift);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = evId });
            }
            catch (Exception)
            {
                SetViewBag(evId);
                ViewBag.Error = "Kunde inte ta bort denna förseningsavgift. Det kan vara så att den används i en registrering.";
                return View(forseningsavgift);
            }
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
