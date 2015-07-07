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
    public class RabatterController : AdminBaseController
    {
        private static string _entity = "Rabatt";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Rabatter
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SetViewBag(id);

            var rabatter = db.Rabatter.Where(r => r.EvenemangsId == id.Value);

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
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SetViewBag(id);

            return View();
        }

        // POST: Rabatter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Kod,Summa,Beskrivning")] Rabatter rabatter, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                rabatter.EvenemangsId = id.Value;
                db.Rabatter.Add(rabatter);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });
            }

            SetViewBag(id);
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

            SetViewBag(id);

            return View(rabatter);
        }

        // POST: Rabatter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Kod,Summa,Beskrivning,EvenemangsId")] Rabatter rabatter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rabatter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = rabatter.EvenemangsId });
            }

            SetViewBag(rabatter.EvenemangsId);
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

            SetViewBag(rabatter.EvenemangsId);

            return View(rabatter);
        }

        // POST: Rabatter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rabatter rabatter = db.Rabatter.Find(id);
            var evId = rabatter.EvenemangsId;
            try
            {
                db.Rabatter.Remove(rabatter);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = evId });
            }
            catch (Exception)
            {
                SetViewBag(evId);
                ViewBag.Error = "Kunde inte ta bort denna rabatt. Det kan vara så att den används i en registrering.";
                return View(rabatter);
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
