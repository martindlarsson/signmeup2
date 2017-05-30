using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;
using SignMeUp2.Helpers;
using SignMeUp2.ViewModels;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class RegistreringarController : BaseController
    {
        private static string _entity = "Registrering";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Registreringar
        public ActionResult Index(int? id)
        {
            IQueryable<Registrering> reggs;
            if (id != null)
            {
                reggs = db.Registreringar.Where(r => r.FormularId == id.Value); //Include(r => r.Bana).Include(r => r.Evenemang).Include(r => r.Kanot).Include(r => r.Klass).Include(r => r.Invoice).Where(r => r.EvenemangsId == id.Value);
                ViewBag.Evenemang = db.Evenemang.FirstOrDefault(e => e.Id == id.Value);
            }
            else if (IsUserAdmin)
            {
                reggs = db.Registreringar.Where(r => r.FormularId == id.Value); //.Include(r => r.Bana).Include(r => r.Evenemang).Include(r => r.Kanot).Include(r => r.Klass);
            }
            else
            {
                return new HttpNotFoundResult();
            }

            return View(reggs.ToList());
        }

        public ActionResult SendInvoice(int? id)
        {
            if (id == null)
            {
                return ShowError(log, "Kan inte skicka faktura utan id", false);
            }
            Registrering registreringar = smuService.GetRegistrering(id.Value, true);

            if (registreringar == null)
            {
                return ShowError(log, "Kan inte hitta registrering med id " + id.Value, false);
            }

            var lyckatsSkicka = false;

            try
            {
                lyckatsSkicka = SkickaFaktura(registreringar);
            } catch (Exception exc)
            {   
                TempData["FelMeddelande"] = "Misslyckades med att skicka faktura. Anledning: " + exc.Message;
                LogError(log, "Misslyckades med att skicka faktura. Anledning: " + exc.Message, exc);
            }

            if (lyckatsSkicka)
            {
                TempData["Meddelande"] = "Faktura skickad till användaren.";
            }

            return RedirectToAction("Index", new { id = registreringar.FormularId });
        }

        // GET: Registreringar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Registrering registrering = smuService.GetRegistrering(id.Value, true);
            var reg = ClassMapper.MappaTillRegistreringVM(registrering);

            if (registrering == null)
            {
                return HttpNotFound();
            }

            ViewBag.FormularsId = reg.FormularId.Value;

            return View(reg);
        }

        // POST: Registreringar/Details/5
        [HttpPost]
        public ActionResult Details(int? id, string sendInvoice, string sendReg)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Registrering registrering = smuService.GetRegistrering(id.Value, true);
            var reg = ClassMapper.MappaTillRegistreringVM(registrering);

            if (registrering == null)
            {
                return HttpNotFound();
            }

            ViewBag.FormularsId = reg.FormularId.Value;

            if (!string.IsNullOrEmpty(sendInvoice)) // TODO testa!!
            {
                SkickaFaktura(registrering);
                log.Debug("Skickat fakturan för reg: " + reg.Id);
                TempData["Message"] = "Fakturan är skickad";
            }

            if (!string.IsNullOrEmpty(sendReg)) // TODO testa!!
            {
                SkickaRegMail(registrering);
                log.Debug("Skickat bekräftelse för reg: " + reg.Id);
                TempData["Message"] = "Bekräftelse är skickad";
            }

            return View(reg);
        }

        // GET: Registreringar/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SetViewBag(id.Value);
            //SetViewBag(null, id.Value);
            return View();
        }

        // POST: Registreringar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Adress,Telefon,Epost,Ranking,Startnummer,Lagnamn,Kanot,Klubb,Klass,HarBetalt,Forseningsavgift,Registreringstid,Kommentar,Bana,Rabatter,PaysonToken,Evenemang_Id,Invoice")] Registrering registreringar)
        {
            if (ModelState.IsValid)
            {
                db.Registreringar.Add(registreringar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //SetViewBag(registrering, registrering.EvenemangsId.Value);
            //SetViewBag(registrering.EvenemangsId);
            return View(registreringar);
        }

        // GET: Registreringar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registrering registrering = smuService.GetRegistrering(id.Value, true);
            var reg = ClassMapper.MappaTillRegistreringVM(registrering);
            if (registrering == null)
            {
                return HttpNotFound();
            }

            //SetViewBag(registrering, registrering.EvenemangsId.Value);
            //SetViewBag(registrering.EvenemangsId);
            return View(reg);
        }

        // POST: Registreringar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
            var regId = form.Get("Id");
            int regIdInt;
            if(!int.TryParse(regId, out regIdInt))
            {
                return ShowError(log, "Fel vid sparande av registrering. Kunde inte parsa regId: " + regId, false);
            }

            var reg = smuService.GetRegistrering(regIdInt, true);

            reg.AttBetala = int.Parse(form.Get("AttBetala"));
            reg.Forseningsavgift = int.Parse(form.Get("Forseningsavgift"));
            reg.HarBetalt = bool.Parse(form.Get("HarBetalt"));

            foreach(var steg in reg.Formular.Steg)
            {
                foreach(var falt in steg.Falt)
                {
                    var varde = form.Get(falt.Id.ToString());
                    var svar = reg.Svar.SingleOrDefault(s => s.FaltId == falt.Id);
                    if (svar != null)
                    {
                        svar.Varde = varde;
                    }
                }
            }

            // TODO invoice!

            db.SaveChanges();

            TempData["Message"] = "Ändringarna är sparade";

            return RedirectToAction("Details", new { id = regIdInt });

            //if (ModelState.IsValid)
            //{
            //    var reg = ClassMapper.MappaTillRegistrering(registrering);
            //    db.Entry(reg).State = EntityState.Modified;

            //    //var origReg = smuService.Db.Registreringar.Include(r => r.Deltagare).First(r => r.Id == registrering.Id);

            //    //foreach (var deltagare in origReg.Deltagare)
            //    //{
            //    //    var nyttFornamn = Request.Form["deltagare_f_" + deltagare.Id];
            //    //    var nyttEfternamn = Request.Form["deltagare_e_" + deltagare.Id];

            //    //    if (!deltagare.Förnamn.Equals(nyttFornamn) || !deltagare.Efternamn.Equals(nyttEfternamn))
            //    //    {
            //    //        deltagare.Förnamn = nyttFornamn;
            //    //        deltagare.Efternamn = nyttEfternamn;
            //    //        db.Entry(deltagare).State = EntityState.Modified;
            //    //    }
            //    //}

            //    if (reg.Invoice != null)
            //    {
            //        db.Entry(reg.Invoice).State = EntityState.Modified;
            //    }

            //    db.SaveChanges();

            //    TempData["Message"] = "Ändringarna är sparade";
            
            //}

            //SetViewBag(registrering, registrering.EvenemangsId.Value);
            //SetViewBag(registrering.EvenemangsId);
            //return View();
        }

        //protected void SetViewBag(Registreringar reg, int evenemangsId)
        //{
        //    if (reg != null)
        //    {
        //        ViewBag.Bana_Id = new SelectList(smuService.HamtaBanor(reg.EvenemangsId.Value), "ID", "Namn", reg.Bana.Id);
        //        ViewBag.Kanot_Id = new SelectList(smuService.HamtaKanoter(reg.EvenemangsId.Value), "ID", "Namn", reg.Kanot.Id);
        //        ViewBag.Klass_Id = new SelectList(smuService.HamtaKlasser(reg.EvenemangsId.Value), "ID", "Namn", reg.Klass.Id);
        //        ViewBag.PlusEllerMinus = EnumHelper.GetSelectList(typeof(TypAvgift));
        //    }
        //    else
        //    {
        //        ViewBag.Bana_Id = new SelectList(smuService.HamtaBanor(evenemangsId), "ID", "Namn");
        //        ViewBag.Kanot_Id = new SelectList(smuService.HamtaKanoter(evenemangsId), "ID", "Namn");
        //        ViewBag.Klass_Id = new SelectList(smuService.HamtaKlasser(evenemangsId), "ID", "Namn");
        //        ViewBag.PlusEllerMinus = EnumHelper.GetSelectList(typeof(TypAvgift));
        //    }
        //}

        // GET: Registreringar/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Registrering registreringar = db.Registreringar.Find(id);
            var formularsId = registreringar.FormularId.Value;
            db.Registreringar.Remove(registreringar);
            db.SaveChanges();

            TempData["Message"] = "Registreringen har tagits bort";
            
            return RedirectToAction("Oversikt", "Formular", new { id = formularsId });
        }

        // POST: Registreringar/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Registrering registreringar = db.Registreringar.Find(id);
        //    db.Registreringar.Remove(registreringar);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
