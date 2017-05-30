using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class OrganisationController : BaseController
    {
        private static string _entity = "Organisation";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Admin/Organisation
        public ActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return View(db.Organisationer.ToList());
            }
            
            var org = HamtaOrganisation();
            return RedirectToAction("Edit", new { id = org.Id });
        }

        // GET: Admin/Organisation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organisation organisation = db.Organisationer.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            return View(organisation);
        }

        // GET: Admin/Organisation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Organisation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Namn,Epost,Adress,AnvändareId,BildUrl")] Organisation organisation)
        {
            if (ModelState.IsValid)
            {
                db.Organisationer.Add(organisation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organisation);
        }

        // GET: Admin/Organisation/Edit/5
        public ActionResult Edit(int? id)
        {
            Organisation organisation = null;
            if (!id.HasValue)
            {
                organisation = HamtaOrganisation();
            }
            else
            {
                organisation = db.Organisationer.Find(id);
            }

            if (organisation == null)
            {
                return HttpNotFound();
            }

            return View(organisation);
        }

        // POST: Admin/Organisation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Namn,Epost,Adress,AnvändareId,BildUrl")] Organisation organisation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organisation).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                ViewBag.FelMeddelande = "Det finns valideringsfel i formuläret. Korrigera och försök igen.";
                return View(organisation);
            }

            ViewBag.Meddelande = "Ändringarna har sparats.";
            
            return View(organisation);
        }

        // GET: Admin/Organisation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organisation organisation = db.Organisationer.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            return View(organisation);
        }

        // POST: Admin/Organisation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Organisation organisation = db.Organisationer.Find(id);
            db.Organisationer.Remove(organisation);
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
