﻿using System;
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
    public class RabatterController : BaseController
    {
        // GET: Rabatter
        public ActionResult Index()
        {
            var rabatter = db.Rabatter.Include(r => r.Evenemang);
            return View(rabatter.ToList());
        }

        // GET: Rabatter/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rabatter rabatter = db.Rabatter.Find(id);
            if (rabatter == null)
            {
                return HttpNotFound();
            }
            return View(rabatter);
        }

        // GET: Rabatter/Create
        public ActionResult Create()
        {
            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn");
            return View();
        }

        // POST: Rabatter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Kod,Summa,Beskrivning,Evenemang_ID")] Rabatter rabatter)
        {
            if (ModelState.IsValid)
            {
                db.Rabatter.Add(rabatter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn", rabatter.Evenemang_ID);
            return View(rabatter);
        }

        // GET: Rabatter/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rabatter rabatter = db.Rabatter.Find(id);
            if (rabatter == null)
            {
                return HttpNotFound();
            }
            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn", rabatter.Evenemang_ID);
            return View(rabatter);
        }

        // POST: Rabatter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Kod,Summa,Beskrivning,Evenemang_ID")] Rabatter rabatter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rabatter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Evenemang_ID = new SelectList(db.Evenemang, "Id", "Namn", rabatter.Evenemang_ID);
            return View(rabatter);
        }

        // GET: Rabatter/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rabatter rabatter = db.Rabatter.Find(id);
            if (rabatter == null)
            {
                return HttpNotFound();
            }
            return View(rabatter);
        }

        // POST: Rabatter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rabatter rabatter = db.Rabatter.Find(id);
            db.Rabatter.Remove(rabatter);
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
