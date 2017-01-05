using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;
using SignMeUp2.Helpers;

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
            Registrering registreringar = smuService.GetRegistrering(id.Value, true);
            if (registreringar == null)
            {
                return HttpNotFound();
            }

            //SetViewBag(registrering, registrering.EvenemangsId.Value);
            //SetViewBag(registrering.EvenemangsId);
            return View(registreringar);
        }

        // POST: Registreringar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Adress,Telefon,Epost,Startnummer,Lagnamn,Kanot_Id,Klubb,Klass_Id,HarBetalt,ForseningsavgiftId,Registreringstid,Kommentar,Bana_Id,RabattId,PaysonToken,EvenemangsId,Invoice")] Registrering registreringar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registreringar).State = EntityState.Modified;

                //var origReg = smuService.Db.Registreringar.Include(r => r.Deltagare).First(r => r.Id == registrering.Id);
                
                //foreach (var deltagare in origReg.Deltagare)
                //{
                //    var nyttFornamn = Request.Form["deltagare_f_" + deltagare.Id];
                //    var nyttEfternamn = Request.Form["deltagare_e_" + deltagare.Id];

                //    if (!deltagare.Förnamn.Equals(nyttFornamn) || !deltagare.Efternamn.Equals(nyttEfternamn))
                //    {
                //        deltagare.Förnamn = nyttFornamn;
                //        deltagare.Efternamn = nyttEfternamn;
                //        db.Entry(deltagare).State = EntityState.Modified;
                //    }
                //}

                if (registreringar.Invoice != null)
                {
                    db.Entry(registreringar.Invoice).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { id = registreringar.FormularId });
            }

            //SetViewBag(registrering, registrering.EvenemangsId.Value);
            //SetViewBag(registrering.EvenemangsId);
            return View(registreringar);
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
            if (registreringar == null)
            {
                return HttpNotFound();
            }
            return View(registreringar);
        }

        // POST: Registreringar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Registrering registreringar = db.Registreringar.Find(id);
            db.Registreringar.Remove(registreringar);
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
