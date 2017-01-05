using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class BetalningsmetoderController : BaseController
    {
        private static string _entity = "Betalningsmetoder";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        public ActionResult CreateOrUpdate()
        {
            var org = HamtaOrganisation();
            ViewBag.OrgEpost = org.Epost;
            return View(org.Betalningsmetoder);
        }

        // POST: Admin/Betalningsmetoder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate([Bind(Include = "GiroTyp,Gironummer,HarPayson,KanTaEmotIntBetalningar,IBAN,BIC")] Betalningsmetoder betalningsmetoder)
        {
            if (ModelState.IsValid)
            {
                var orgId = HamtaUser().OrganisationsId;
                var org = db.Organisationer.Find(orgId);

                var orgBetMetod = org.Betalningsmetoder;

                if (orgBetMetod != null)
                {
                    betalningsmetoder.Id = orgBetMetod.Id;
                }

                org.Betalningsmetoder = betalningsmetoder;
                
                db.SaveChanges();

                TempData["Message"] = "Ändringarna är sparade";
            }

            else
            {
                ShowError(log, "Det gick inte att spara ändringen.", false);
            }


            return View(betalningsmetoder);
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
