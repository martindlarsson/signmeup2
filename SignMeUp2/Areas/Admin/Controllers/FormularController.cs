using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using SignMeUp2.Controllers;
using SignMeUp2.Helpers;
using System.Web.Script.Serialization;
using System.Web.Helpers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class FormularController : BaseController
    {
        // GET: Admin/Formular/Oversikt
        public ActionResult Oversikt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formular formular = db.Formular.Include("Aktivitet").Single(f => f.Id == id.Value);
            if (formular == null)
            {
                return HttpNotFound();
            }
            var formularVM = ClassMapper.MappaTillFormularVM(formular);
            //formularVM.Aktiviteter = smuService.GetAktiviteter();
            SetViewBag(formularVM.EvenemangsId);
            return View(formularVM);
        }

        // GET: Admin/Formular/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Formular/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EvenemangsId,Namn,Avgift")] FormularViewModel formularVM)
        {
            if (ModelState.IsValid)
            {
                Formular formular = ClassMapper.MappaTillFormular(formularVM);
                db.Formular.Add(formular);
                db.SaveChanges();
                db.Entry(formular).GetDatabaseValues();
                return RedirectToAction("Oversikt", "Formular", new { id = formular.Id });
            }

            SetViewBag(formularVM.EvenemangsId);
            return View(formularVM);
        }

        // GET: Admin/Formular/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formular formular = db.Formular.Find(id);
            if (formular == null)
            {
                return HttpNotFound();
            }
            var formularVM = ClassMapper.MappaTillFormularVM(formular);
            formularVM.Aktiviteter = smuService.GetAktiviteter();
            SetViewBag(formularVM.EvenemangsId);
            return View(formularVM);
        }

        // POST: Admin/Formular/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EvenemangsId,Namn,Avgift,AktivitetsId,MaxRegistreringar,Publikt,AnnanAktivitet")] FormularViewModel formularVM)
        {
            if (ModelState.IsValid)
            {
                if (formularVM.AktivitetsId == 1 && !string.IsNullOrEmpty(formularVM.AnnanAktivitet))
                {
                    var nyAktivitet = new Aktivitet { Namn = formularVM.AnnanAktivitet };
                    db.Aktiviteter.Add(nyAktivitet);
                    db.SaveChanges();
                    db.Entry(nyAktivitet).GetDatabaseValues();
                    formularVM.AktivitetsId = nyAktivitet.Id;
                }

                var formular = ClassMapper.MappaTillFormular(formularVM);
                db.Entry(formular).State = EntityState.Modified;
                
                // TODO läs in eventuella registreringar, steg, och listor så att de inte nollställs
                db.SaveChanges();
                TempData["Message"] = "Formuläret uppdaterades.";
                return RedirectToAction("Oversikt", "Formular", new { id = formular.Id });
            }

            formularVM.Aktiviteter = smuService.GetAktiviteter();
            SetViewBag(formularVM.EvenemangsId);
            return View(formularVM);
        }

        // GET: Admin/Formular/Delete/5
        public ActionResult Delete(int? id)
        {
            Formular formular = db.Formular.Find(id);
            var evenemangsId = formular.EvenemangsId;
            var fNamn = formular.Namn;
            db.Formular.Remove(formular);
            db.SaveChanges();
            TempData["Message"] = "Formulär med namn " + fNamn + " har raderats.";
            return RedirectToAction("Oversikt", "Evenemang", new { Id = evenemangsId });
        }

        public ActionResult Formularsbyggare(int? id)
        {
            // TODO ladda formularet
            //Formular formular = db.Formular.Find(id);
            //var json = new JavaScriptSerializer().Serialize(formular);
            //return View(json);
            return View(Json(new { formularsId = 1 }));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override string GetEntitetsNamn()
        {
            return "Formulär";
        }
    }
}
