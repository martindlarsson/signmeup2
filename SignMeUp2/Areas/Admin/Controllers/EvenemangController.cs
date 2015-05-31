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
    public class EvenemangController : AdminBaseController
    {
        // GET: Evenemang
        public ActionResult Index()
        {
            return View(HamtaEvenemangForAnv());
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
        public ActionResult Create([Bind(Include = "Id,Namn,RegStart,RegStop,AntalDeltagare")] Evenemang evenemang)
        {
            if (ModelState.IsValid)
            {
                evenemang.OrganisationsId = GetUser().OrganisationsId;
                db.Evenemang.Add(evenemang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
            return View(evenemang);
        }

        // POST: Evenemang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Namn,RegStart,RegStop,AntalDeltagare")] Evenemang evenemang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evenemang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
            evenemang.Registreringar.Clear();
            //foreach (Registreringar reg in evenemang.Registreringar)
            //{
            //    evenemang.Registreringar.Remove(reg);
            //}
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
