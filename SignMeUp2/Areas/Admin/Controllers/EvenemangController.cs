using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Controllers;
using SignMeUp2.Helpers;
using System.Collections.Generic;

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
        public ActionResult Create([Bind(Include = "Id,Namn,RegStart,RegStop,Fakturabetalning,FakturaBetaldSenast,Språk")] ViewModels.EvenemangVM evenemang)
        {
            if (ModelState.IsValid)
            {
                evenemang.OrganisationsId = HamtaUser().OrganisationsId;

                var nyttEvenemang = ClassMapper.MappaTillEvenemang(evenemang);

                var formular = new Formular
                {
                    Avgift = 0,
                    Namn = "Anmälan till " + evenemang.Namn,
                    AktivitetsId = 2
                };

                var formularSteg1 = new FormularSteg
                {
                    Index = 0,
                    Namn = "Kontaktuppgifter målsman"
                };
                formular.Steg.Add(formularSteg1);

                var faltMålsman = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "För och efternamn",
                    Typ = FaltTyp.text_falt
                };
                formularSteg1.Falt.Add(faltMålsman);

                var faltPerNummer = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Personnummer",
                    Typ = FaltTyp.text_falt
                };
                formularSteg1.Falt.Add(faltPerNummer);

                var faltAdress =new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Adress",
                    Typ = FaltTyp.text_falt
                };
                formularSteg1.Falt.Add(faltAdress);

                var faltEpost = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Epost",
                    Typ = FaltTyp.epost_falt
                };
                formularSteg1.Falt.Add(faltEpost);

                var faltMobil = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Mobilnummer",
                    Typ = FaltTyp.text_falt
                };
                formularSteg1.Falt.Add(faltMobil);
                

                var formularSteg2 = new FormularSteg
                {
                    Index = 1,
                    Namn = "Deltagare"
                };
                formular.Steg.Add(formularSteg2);

                var faltJ1Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Junior 1 namn",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ1Namn);

                var faltJ1Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Junior 1 personnummer",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ1Pers);

                var faltJ2Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Junior 2 namn",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ2Namn);

                var faltJ2Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Junior 2 personnummer",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ2Pers);

                var faltJ3Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Junior 3 namn",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ3Namn);

                var faltJ3Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Junior 3 personnummer",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ3Pers);

                var faltJ4Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Junior 4 namn",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ4Namn);

                var faltJ4Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Junior 4 personnummer",
                    Typ = FaltTyp.text_falt
                };
                formularSteg2.Falt.Add(faltJ4Pers);

                

                nyttEvenemang.Formular.Add(formular);

                db.Evenemang.Add(nyttEvenemang);
                db.SaveChanges();
                db.Entry(nyttEvenemang).GetDatabaseValues();


                var startlista = new Lista
                {
                    Namn = "Anmälda till " + evenemang.Namn
                };

                startlista.Falt = new List<ListaFalt>();

                startlista.Falt.Add(new ListaFalt { Falt = faltMålsman, Index = 0, Alias = "Målsman" });
                startlista.Falt.Add(new ListaFalt { Falt = faltEpost, Index = 1, Alias = "Epost" });
                startlista.Falt.Add(new ListaFalt { Falt = faltJ1Namn, Index = 2, Alias = "J1" });
                startlista.Falt.Add(new ListaFalt { Falt = faltJ2Namn, Index = 3, Alias = "J2" });
                startlista.Falt.Add(new ListaFalt { Falt = faltJ3Namn, Index = 4, Alias = "J3" });
                startlista.Falt.Add(new ListaFalt { Falt = faltJ4Namn, Index = 5, Alias = "J4" });

                formular.Listor = new List<Lista>();
                formular.Listor.Add(startlista);

                db.SaveChanges();


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
        public ActionResult Edit([Bind(Include = "Id,Namn,RegStart,RegStop,Fakturabetalning,FakturaBetaldSenast,Språk")] Evenemang evenemang)
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
