using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class DeltagareController : BaseController
    {
        private static string _entity = "Deltagare";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Deltagare
        public ActionResult Index()
        {
            var deltagare = db.Deltagare.Include(d => d.Registreringar);
            return View(deltagare.ToList());
        }

        // GET: Deltagare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deltagare deltagare = db.Deltagare.Find(id);
            if (deltagare == null)
            {
                return HttpNotFound();
            }
            return View(deltagare);
        }

        // GET: Deltagare/Create
        public ActionResult Create()
        {
            ViewBag.RegistreringarID = new SelectList(db.Registreringar, "ID", "Adress");
            return View();
        }

        // POST: Deltagare/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Förnamn,Efternamn,Personnummer,RegistreringarID")] Deltagare deltagare)
        {
            if (ModelState.IsValid)
            {
                db.Deltagare.Add(deltagare);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RegistreringarID = new SelectList(db.Registreringar, "ID", "Adress", deltagare.RegistreringarID);
            return View(deltagare);
        }

        // GET: Deltagare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deltagare deltagare = db.Deltagare.Find(id);
            if (deltagare == null)
            {
                return HttpNotFound();
            }
            ViewBag.RegistreringarID = new SelectList(db.Registreringar, "ID", "Adress", deltagare.RegistreringarID);
            return View(deltagare);
        }

        // POST: Deltagare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Förnamn,Efternamn,Personnummer,RegistreringarID")] Deltagare deltagare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deltagare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegistreringarID = new SelectList(db.Registreringar, "ID", "Adress", deltagare.RegistreringarID);
            return View(deltagare);
        }

        // GET: Deltagare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deltagare deltagare = db.Deltagare.Find(id);
            if (deltagare == null)
            {
                return HttpNotFound();
            }
            return View(deltagare);
        }

        // POST: Deltagare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deltagare deltagare = db.Deltagare.Find(id);
            db.Deltagare.Remove(deltagare);
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
