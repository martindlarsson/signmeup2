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
    public class BanorController : Controller
    {
        private SignMeUpDataModel db = new SignMeUpDataModel();

        // GET: Admin/Banor
        public ActionResult Index()
        {
            var banor = db.Banor.Include(b => b.Evenemang);
            return View(banor.ToList());
        }

        // GET: Admin/Banor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banor banor = db.Banor.Find(id);
            if (banor == null)
            {
                return HttpNotFound();
            }
            return View(banor);
        }

        // GET: Admin/Banor/Create
        public ActionResult Create()
        {
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn");
            return View();
        }

        // POST: Admin/Banor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Namn,Avgift,AntalDeltagare,EvenemangsId")] Banor banor)
        {
            if (ModelState.IsValid)
            {
                db.Banor.Add(banor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", banor.EvenemangsId);
            return View(banor);
        }

        // GET: Admin/Banor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banor banor = db.Banor.Find(id);
            if (banor == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", banor.EvenemangsId);
            return View(banor);
        }

        // POST: Admin/Banor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Namn,Avgift,AntalDeltagare,EvenemangsId")] Banor banor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(banor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", banor.EvenemangsId);
            return View(banor);
        }

        // GET: Admin/Banor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banor banor = db.Banor.Find(id);
            if (banor == null)
            {
                return HttpNotFound();
            }
            return View(banor);
        }

        // POST: Admin/Banor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banor banor = db.Banor.Find(id);
            db.Banor.Remove(banor);
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
