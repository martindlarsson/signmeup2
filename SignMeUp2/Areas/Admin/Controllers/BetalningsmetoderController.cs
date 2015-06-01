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
    public class BetalningsmetoderController : AdminBaseController
    {
        public ActionResult CreateOrUpdate()
        {
            var org = HamtaOrganisation();

            if (org.Betalningsmetoder == null)
            {
                return RedirectToAction("create");
            }
            else
            {
                return RedirectToAction("edit", new { id = org.Betalningsmetoder.Id });
            }
        }

        // GET: Admin/Betalningsmetoder
        public ActionResult Index()
        {
            return View(db.Betalningsmetoders.ToList());
        }

        // GET: Admin/Betalningsmetoder/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Betalningsmetoder betalningsmetoder = db.Betalningsmetoders.Find(id);
            if (betalningsmetoder == null)
            {
                return HttpNotFound();
            }
            return View(betalningsmetoder);
        }

        // GET: Admin/Betalningsmetoder/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Betalningsmetoder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GiroTyp,Gironummer,PaysonUserId,PaysonUserKey")] Betalningsmetoder betalningsmetoder)
        {
            if (ModelState.IsValid)
            {
                //db.Betalningsmetoders.Add(betalningsmetoder);
                var orgId = HamtaUser().OrganisationsId;
                var org = db.Organisationer.Find(orgId);
                org.Betalningsmetoder = betalningsmetoder;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(betalningsmetoder);
        }

        // GET: Admin/Betalningsmetoder/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Betalningsmetoder betalningsmetoder = db.Betalningsmetoders.Find(id);
            if (betalningsmetoder == null)
            {
                return HttpNotFound();
            }
            return View(betalningsmetoder);
        }

        // POST: Admin/Betalningsmetoder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GiroTyp,Gironummer,PaysonUserId,PaysonUserKey")] Betalningsmetoder betalningsmetoder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(betalningsmetoder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(betalningsmetoder);
        }

        // GET: Admin/Betalningsmetoder/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Betalningsmetoder betalningsmetoder = db.Betalningsmetoders.Find(id);
            if (betalningsmetoder == null)
            {
                return HttpNotFound();
            }
            return View(betalningsmetoder);
        }

        // POST: Admin/Betalningsmetoder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Betalningsmetoder betalningsmetoder = db.Betalningsmetoders.Find(id);
            db.Betalningsmetoders.Remove(betalningsmetoder);
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
