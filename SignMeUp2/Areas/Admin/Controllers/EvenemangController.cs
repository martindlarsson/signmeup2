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
    public class EvenemangController : BaseController
    {
        private static string _entity = "Evenemang";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        public ActionResult Oversikt(int? id)
        {
            var evenemang = db.Evenemang.Include("Rabatter").Include("Forseningsavgifter").Include("Formular").Include("Formular.Aktivitet").FirstOrDefault(e => e.Id == id.Value);

            if (evenemang == null)
                return ShowError(log, "Hittade inte evenemanget", true);

            return View(evenemang);
        }

        // GET: Evenemang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Evenemang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Namn,RegStart,RegStop,Fakturabetalning,FakturaBetaldSenast")] ViewModels.EvenemangVM evenemang)
        {
            if (ModelState.IsValid)
            {
                evenemang.OrganisationsId = HamtaUser().OrganisationsId;

                var nyttEvenemang = ClassMapper.MappaTillEvenemang(evenemang);

                var formular = new Formular
                {
                    Avgift = 100,
                    Namn = "Mitt första formulär",
                    AktivitetsId = 2
                };
                var formularSteg1 = new FormularSteg
                {
                    Index = 0,
                    Namn = "Kontaktuppgifter"
                };
                var formularSteg2 = new FormularSteg
                {
                    Index = 1,
                    Namn = "Tävling"
                };
                var falt1 = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "För och efternamn",
                    Typ = FaltTyp.text_falt
                };
                var falt2 = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Epost",
                    Typ = FaltTyp.epost_falt
                };
                var falt3 = new Falt
                {
                    Avgiftsbelagd = true,
                    Kravs = true,
                    Namn = "Bana",
                    Typ = FaltTyp.val_falt
                };
                var val1 = new Val
                {
                    Avgift = 50,
                    Namn = "Korta banan (5 km)",
                    TypNamn = "Bana"
                };
                var val2 = new Val
                {
                    Avgift = 100,
                    Namn = "Långa banan (10 km)",
                    TypNamn = "Bana"
                };
                falt3.Val.Add(val1);
                falt3.Val.Add(val2);
                formularSteg1.Falt.Add(falt1);
                formularSteg1.Falt.Add(falt2);
                formularSteg2.Falt.Add(falt3);
                formular.Steg.Add(formularSteg1);
                formular.Steg.Add(formularSteg2);
                nyttEvenemang.Formular.Add(formular);

                db.Evenemang.Add(nyttEvenemang);
                db.SaveChanges();
                db.Entry(nyttEvenemang).GetDatabaseValues();
                return RedirectToAction("Oversikt", new { id = nyttEvenemang.Id });
            }

            ViewBag.FelMeddelande = "Det finns valideringsfel i formuläret. Korrigera och försök igen.";

            return View(evenemang);
        }

        // GET: Evenemang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evenemang evenemang = db.Evenemang.Find(id);
            if (evenemang == null)
            {
                return HttpNotFound();
            }

            SetViewBag(id);

            return View(evenemang);
        }

        // POST: Evenemang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Namn,RegStart,RegStop,Fakturabetalning,FakturaBetaldSenast")] Evenemang evenemang)
        {
            if (ModelState.IsValid)
            {
                var user = HamtaUser();
                evenemang.OrganisationsId = user.OrganisationsId;
                db.Entry(evenemang).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                ViewBag.FelMeddelande = "Det finns valideringsfel i formuläret. Korrigera och försök igen.";
                SetViewBag(evenemang);
                return View(evenemang);
            }

            ViewBag.Meddelande = "Ändringarna har sparats.";

            SetViewBag(evenemang);
            return View(evenemang);
        }

        // GET: Evenemang/Delete/5
        public ActionResult Delete(int? id)
        {
            Evenemang evenemang = db.Evenemang.Find(id);
            var namn = evenemang.Namn;

            db.Evenemang.Remove(evenemang);
            db.SaveChanges();

            TempData["Message"] = "Evenemang " + namn + " har tagits bort";

            return RedirectToAction("Index", "Admin", null);
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
