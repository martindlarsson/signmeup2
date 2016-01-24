using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SignMeUp2.Data;
using SignMeUp2.Controllers;
using SignMeUp2.ViewModels;
using SignMeUp2.Helpers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class ForseningsavgiftController : BaseController
    {
        private static string _entity = "Förseningsavgift";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Admin/Forseningsavgift/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.PlusEllerMinus = EnumHelper.GetSelectList(typeof(TypAvgift));
            SetViewBag(id);
            
            return View();
        }

        // POST: Admin/Forseningsavgift/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Namn,FranDatum,TillDatum,PlusEllerMinus,Summa")] ForseningsavgiftVM forseningsavgift, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            forseningsavgift.EvenemangsId = id.Value;
            
            if (ModelState.IsValid)
            {
                var nyForseningsavgift = ClassMapper.MappaTillForseningsavgift(forseningsavgift);
                db.Forseningsavgift.Add(nyForseningsavgift);
                db.SaveChanges();
                return RedirectToAction("Oversikt", "Evenemang", new { id = id });
            }

            ViewBag.PlusEllerMinus = EnumHelper.GetSelectList(typeof(TypAvgift));
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

            var forseningsavgVM = ClassMapper.MappaTillForseningsavgiftVM(forseningsavgift);

            ViewBag.PlusEllerMinus = EnumHelper.GetSelectList(typeof(TypAvgift));
            SetViewBag(forseningsavgift.EvenemangsId);

            return View(forseningsavgVM);
        }

        // POST: Admin/Forseningsavgift/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Namn,FranDatum,TillDatum,PlusEllerMinus,Summa,EvenemangsId")] ForseningsavgiftVM forseningsavgiftVM)
        {
            if (ModelState.IsValid)
            {
                var forseningsavg = ClassMapper.MappaTillForseningsavgift(forseningsavgiftVM);
                db.Entry(forseningsavg).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Sparat ändringar i förseningsavgift med namn " + forseningsavgiftVM.Namn;
                return RedirectToAction("Oversikt", "Evenemang", new { id = forseningsavgiftVM.EvenemangsId });
            }

            return View(forseningsavgiftVM);
        }

        // GET: Admin/Forseningsavgift/Delete/5
        public ActionResult Delete(int? id)
        {
            Forseningsavgift forseningsavgift = db.Forseningsavgift.Find(id);
            var evId = forseningsavgift.EvenemangsId;

            try
            {
                db.Forseningsavgift.Remove(forseningsavgift);
                db.SaveChanges();
                TempData["Message"] = "Raderat förseningsavgift med namn " + forseningsavgift.Namn;
                return RedirectToAction("Oversikt", "Evenemang", new { id = evId });
            }
            catch (Exception)
            {
                SetViewBag(evId);
                ViewBag.Error = "Kunde inte ta bort denna förseningsavgift.";
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
