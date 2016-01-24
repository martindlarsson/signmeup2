using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;
using SignMeUp2.ViewModels;
using SignMeUp2.Helpers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class RabatterController : BaseController
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
        public ActionResult Create([Bind(Include = "Id,Kod,Summa,Beskrivning")] RabattVM rabatt, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                Rabatter nyRabatt = ClassMapper.MappaTillRabatt(rabatt);
                nyRabatt.EvenemangsId = id.Value;
                db.Rabatter.Add(nyRabatt);
                db.SaveChanges();
                return RedirectToAction("Oversikt", "Evenemang", new { id = id });
            }

            SetViewBag(id);
            return View(rabatt);
        }

        // GET: Rabatter/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rabatter rabatter = db.Rabatter.Find(id);

            RabattVM rabatt = ClassMapper.MappaTillRabattVM(rabatter);

            if (rabatter == null)
            {
                return HttpNotFound();
            }

            SetViewBag(rabatt.EvenemangsId);

            return View(rabatt);
        }

        // POST: Rabatter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Kod,Summa,Beskrivning,EvenemangsId")] RabattVM rabatter)
        {
            if (ModelState.IsValid)
            {
                var editeradRabatt = ClassMapper.MappaTillRabatt(rabatter);
                db.Entry(editeradRabatt).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Sparat ändringar i rabatt med kod " + editeradRabatt.Kod;
                return RedirectToAction("Oversikt", "Evenemang", new { id = rabatter.EvenemangsId });
            }

            SetViewBag(rabatter.EvenemangsId);
            return View(rabatter);
        }

        // GET: Rabatter/Delete/5
        public ActionResult Delete(int? id)
        {
            Rabatter rabatter = db.Rabatter.Find(id);
            var evId = rabatter.EvenemangsId;
            var kod = rabatter.Kod;
            try
            {
                db.Rabatter.Remove(rabatter);
                db.SaveChanges();
                TempData["Message"] = "Raderat rabatt med kod " + kod;
                return RedirectToAction("Oversikt", "Evenemang", new { id = evId });
            }
            catch (Exception)
            {
                SetViewBag(evId);
                ViewBag.Error = "Kunde inte ta bort denna rabatt. Det kan vara så att den används i en registrering.";
                return View(rabatter);
            }
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
