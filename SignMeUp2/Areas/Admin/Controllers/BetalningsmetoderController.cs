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
        private static string _entity = "Betalningsmetoder";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        public ActionResult CreateOrUpdate()
        {
            var org = HamtaOrganisation();

            return View(org.Betalningsmetoder);
        }

        // POST: Admin/Betalningsmetoder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate([Bind(Include = "ID,GiroTyp,Gironummer,PaysonUserId,PaysonUserKey,HarPayson,KanTaEmotIntBetalningar,IBAN,BIC")] Betalningsmetoder betalningsmetoder)
        {
            if (ModelState.IsValid)
            {
                // Edit
                if (betalningsmetoder.Id != 0)
                {
                    db.Entry(betalningsmetoder).State = EntityState.Modified;
                }
                // Create
                else
                {
                    var orgId = HamtaUser().OrganisationsId;
                    var org = db.Organisationer.Find(orgId);
                    org.Betalningsmetoder = betalningsmetoder;
                }
                db.SaveChanges();

                // TODO sätt en flagga i ViewBag att ändringarna är sparade
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
                //return RedirectToAction("Index");
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
