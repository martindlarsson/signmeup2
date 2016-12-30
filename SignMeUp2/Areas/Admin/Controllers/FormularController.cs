using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using SignMeUp2.Controllers;
using SignMeUp2.Helpers;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class FormularController : BaseController
    {
        private static string _entity = "Formulär";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Admin/Formular/Oversikt
        public ActionResult Oversikt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formular formular = db.Formular.Include("Aktivitet").Include("Registreringar.Invoice").Single(f => f.Id == id.Value);
            if (formular == null)
            {
                return HttpNotFound();
            }
            var formularVM = ClassMapper.MappaTillFormularVM(formular);
            //formularVM.Aktiviteter = smuService.GetAktiviteter();
            //ViewBag.Registreringar = formular.Registreringar.Select(regg => ClassMapper.MappaTillRegistreringVM(regg)).ToList();

            ViewBag.RegTable = GenerateRegistrationsTable(formular);

            SetViewBag(formularVM.EvenemangsId);
            return View(formularVM);
        }

        internal TabellViewModel GenerateRegistrationsTable(Formular formular)
        {
            var table = new TabellViewModel();
            table.Kolumner = new List<Kolumn>();
            table.Kolumner.Add(new Kolumn { Rubrik = "Registreringstid" });
            table.Kolumner.Add(new Kolumn { Rubrik = "E-post" });
            table.Kolumner.Add(new Kolumn { Rubrik = "Betalningsmetod" });
            table.Kolumner.Add(new Kolumn { Rubrik = "Att betala" });
            table.Kolumner.Add(new Kolumn { Rubrik = "Betalt" });

            table.Rader = new List<Rad>();
            foreach(var reg in formular.Registreringar)
            {
                var varden = new List<String>();
                varden.Add("<a href=\"" + Url.Action("Details", "Registreringar", new { id = reg.Id }) + "\">" + reg.Registreringstid.ToString() + "</a>");
                var epost = "";
                foreach(var steg in reg.Formular.Steg)
                {
                    foreach(var falt in steg.Falt)
                    {
                        if (falt.Typ == FaltTyp.epost_falt)
                        {
                            var svar = reg.Svar.Single(s => s.FaltId == falt.Id);
                            epost += svar.Varde + " ";
                        }
                    }
                }
                varden.Add(epost);
                varden.Add(reg.Invoice == null ? "Payson" : "Faktura");
                varden.Add(reg.AttBetala.ToString());
                varden.Add(reg.HarBetalt ? "Ja" : "Nej");
                // TODO epost

                table.Rader.Add(new Rad { Varden = varden });
            }

            return table;
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
            Formular formular = db.Formular.Find(id);
            if (formular == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var formularVM = ClassMapper.MappaTillFormularVM(formular);
            //var fVM = new { id = formular.Id, namn = formular.Namn };
            var json = new JavaScriptSerializer().Serialize(formularVM);
            //return View(json);
            TempData["formular"] = json;
            return View();
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
