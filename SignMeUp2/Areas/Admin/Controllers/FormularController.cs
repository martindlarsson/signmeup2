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
    public class FormularController : BaseController
    {
        // GET: Admin/Formular
        public ActionResult Index(int? Id)
        {
            ICollection<Formular> formular = smuService.GetFormularForEvenemang(Id.Value);
            return View(formular);
        }

        // GET: Admin/Formular/Create
        public ActionResult Create()
        {
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn");
            return View();
        }

        // POST: Admin/Formular/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EvenemangsId,Namn,Avgift")] Formular formular)
        {
            if (ModelState.IsValid)
            {
                db.Formular.Add(formular);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", formular.EvenemangsId);
            return View(formular);
        }

        // GET: Admin/Formular/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formular formular = db.Formular.Find(id);
            if (formular == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", formular.EvenemangsId);
            return View(formular);
        }

        // POST: Admin/Formular/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EvenemangsId,Namn,Avgift")] Formular formular)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formular).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvenemangsId = new SelectList(db.Evenemang, "Id", "Namn", formular.EvenemangsId);
            return View(formular);
        }

        // GET: Admin/Formular/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formular formular = db.Formular.Find(id);
            if (formular == null)
            {
                return HttpNotFound();
            }
            return View(formular);
        }

        // POST: Admin/Formular/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Formular formular = db.Formular.Find(id);
            db.Formular.Remove(formular);
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

        protected override string GetEntitetsNamn()
        {
            return "Formulär";
        }
    }
}
