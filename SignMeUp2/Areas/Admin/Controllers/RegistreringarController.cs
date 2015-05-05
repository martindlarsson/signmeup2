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
    public class RegistreringarController : BaseController
    {
        // GET: Registreringar
        public ActionResult Index()
        {
            var registreringar = db.Registreringar.Include(r => r.Banor).Include(r => r.Evenemang).Include(r => r.Kanoter).Include(r => r.Klasser);
            return View(registreringar.ToList());
        }

        // GET: Registreringar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registreringar registreringar = db.Registreringar.Find(id);
            if (registreringar == null)
            {
                return HttpNotFound();
            }
            return View(registreringar);
        }

        // GET: Registreringar/Create
        public ActionResult Create()
        {
            ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn");
            ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn");
            ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn");
            ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn");
            return View();
        }

        // POST: Registreringar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Adress,Telefon,Epost,Ranking,Startnummer,Lagnamn,Kanot,Klubb,Klass,HarBetalt,Forseningsavgift,Registreringstid,Kommentar,Bana,Rabatter,PaysonToken,Evenemang_Id,Invoice")] Registreringar registreringar)
        {
            if (ModelState.IsValid)
            {
                db.Registreringar.Add(registreringar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn", registreringar.Bana);
            ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn", registreringar.Evenemang_Id);
            ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn", registreringar.Kanot);
            ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn", registreringar.Klass);
            return View(registreringar);
        }

        // GET: Registreringar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registreringar registreringar = db.Registreringar.Find(id);
            if (registreringar == null)
            {
                return HttpNotFound();
            }
            ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn", registreringar.Bana);
            ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn", registreringar.Evenemang_Id);
            ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn", registreringar.Kanot);
            ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn", registreringar.Klass);
            return View(registreringar);
        }

        // POST: Registreringar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Adress,Telefon,Epost,Ranking,Startnummer,Lagnamn,Kanot,Klubb,Klass,HarBetalt,Forseningsavgift,Registreringstid,Kommentar,Bana,Rabatter,PaysonToken,Evenemang_Id,Invoice")] Registreringar registreringar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registreringar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Bana = new SelectList(db.Banor, "ID", "Namn", registreringar.Bana);
            ViewBag.Evenemang_Id = new SelectList(db.Evenemang, "Id", "Namn", registreringar.Evenemang_Id);
            ViewBag.Kanot = new SelectList(db.Kanoter, "ID", "Namn", registreringar.Kanot);
            ViewBag.Klass = new SelectList(db.Klasser, "ID", "Namn", registreringar.Klass);
            return View(registreringar);
        }

        // GET: Registreringar/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registreringar registreringar = db.Registreringar.Find(id);
            if (registreringar == null)
            {
                return HttpNotFound();
            }
            return View(registreringar);
        }

        // POST: Registreringar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Registreringar registreringar = db.Registreringar.Find(id);
            db.Registreringar.Remove(registreringar);
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
