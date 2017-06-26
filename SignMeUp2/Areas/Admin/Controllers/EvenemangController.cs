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
                    Namn = "Registration for " + evenemang.Namn,
                    AktivitetsId = 2
                };

                var formularSteg1 = new FormularSteg
                {
                    Index = 0,
                    Namn = "Course fee address"
                };
                formular.Steg.Add(formularSteg1);

                var faltMålsman = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "First and last name",
                    Typ = FaltTyp.text_falt,
                    Index = 0
                };
                formularSteg1.Falt.Add(faltMålsman);

                var faltKlubb = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Ski club",
                    Typ = FaltTyp.text_falt,
                    Index = 1
                };
                formularSteg1.Falt.Add(faltKlubb);

                var faltPerNummer = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Personal id",
                    Typ = FaltTyp.text_falt,
                    Index = 2
                };
                formularSteg1.Falt.Add(faltPerNummer);

                var faltAdress =new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Address",
                    Typ = FaltTyp.text_falt,
                    Index = 3
                };
                formularSteg1.Falt.Add(faltAdress);

                var faltEpost = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Email",
                    Typ = FaltTyp.epost_falt,
                    Index = 4
                };
                formularSteg1.Falt.Add(faltEpost);

                var faltMobil = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Cellphone",
                    Typ = FaltTyp.text_falt,
                    Index = 5
                };
                formularSteg1.Falt.Add(faltMobil);

                var faltInfo = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "Please add countrycode if other than Sweden. Ie +47 for Norway.",
                    Typ = FaltTyp.info_falt,
                    Index = 6
                };
                formularSteg1.Falt.Add(faltInfo);


                var formularSteg2 = new FormularSteg
                {
                    Index = 1,
                    Namn = "Racers"
                };
                formular.Steg.Add(formularSteg2);

                var faltJ1Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "Racer 1 (R1) name",
                    Typ = FaltTyp.text_falt,
                    Index = 0
                };
                formularSteg2.Falt.Add(faltJ1Namn);

                var faltJ1Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = true,
                    Namn = "R1 personal id",
                    Typ = FaltTyp.text_falt,
                    Index = 1
                };
                formularSteg2.Falt.Add(faltJ1Pers);

                var faltJ2Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R2 name",
                    Typ = FaltTyp.text_falt,
                    Index = 5
                };
                formularSteg2.Falt.Add(faltJ2Namn);

                var faltJ2Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R2 personal id",
                    Typ = FaltTyp.text_falt,
                    Index = 6
                };
                formularSteg2.Falt.Add(faltJ2Pers);

                var faltJ2Family = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R2 is a family member",
                    Typ = FaltTyp.val_falt,
                    Index = 7,
                    Val = new List<Val> {
                        new Val { Avgift = 0, Namn = "Yes" },
                        new Val { Avgift = 0, Namn = "No" },
                    }
                };
                formularSteg2.Falt.Add(faltJ2Family);

                var faltJ3Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R3 name",
                    Typ = FaltTyp.text_falt,
                    Index = 9
                };
                formularSteg2.Falt.Add(faltJ3Namn);

                var faltJ3Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R3 personal id",
                    Typ = FaltTyp.text_falt,
                    Index = 10
                };
                formularSteg2.Falt.Add(faltJ3Pers);

                var faltJ3Family = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R3 is a family member",
                    Typ = FaltTyp.val_falt,
                    Index = 11,
                    Val = new List<Val> {
                        new Val { Avgift = 0, Namn = "Yes" },
                        new Val { Avgift = 0, Namn = "No" },
                    }
                };
                formularSteg2.Falt.Add(faltJ3Family);

                var faltJ4Namn = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R4 name",
                    Typ = FaltTyp.text_falt,
                    Index = 13
                };
                formularSteg2.Falt.Add(faltJ4Namn);

                var faltJ4Pers = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R4 personal id",
                    Typ = FaltTyp.text_falt,
                    Index = 14
                };
                formularSteg2.Falt.Add(faltJ4Pers);

                var faltJ4Family = new Falt
                {
                    Avgiftsbelagd = false,
                    Kravs = false,
                    Namn = "R4 is a family member",
                    Typ = FaltTyp.val_falt,
                    Index = 15,
                    Val = new List<Val> {
                        new Val { Avgift = 0, Namn = "Yes" },
                        new Val { Avgift = 0, Namn = "No" },
                    }
                };
                formularSteg2.Falt.Add(faltJ4Family);



                nyttEvenemang.Formular.Add(formular);

                db.Evenemang.Add(nyttEvenemang);
                db.SaveChanges();
                db.Entry(nyttEvenemang).GetDatabaseValues();


                var startlista = new Lista
                {
                    Namn = "Ragistrations for " + evenemang.Namn,
                    Falt = new List<ListaFalt>
                    {
                        new ListaFalt { Falt = faltMålsman, Index = 0, Alias = "Invoice name" },
                        new ListaFalt { Falt = faltEpost, Index = 1, Alias = "Email" },
                        new ListaFalt { Falt = faltJ1Namn, Index = 2, Alias = "R1" },
                        new ListaFalt { Falt = faltJ2Namn, Index = 3, Alias = "R2" },
                        new ListaFalt { Falt = faltJ3Namn, Index = 4, Alias = "R3" },
                        new ListaFalt { Falt = faltJ4Namn, Index = 5, Alias = "R4" },
                    }
                };
                
                var epostlista = new Lista
                {
                    Namn = "Epost for " + evenemang.Namn,
                    Falt = new List<ListaFalt>
                    {
                        new ListaFalt { Falt = faltEpost, Index = 0, Alias = "Email" }
                    }
                };

                formular.Listor = new List<Lista>();
                formular.Listor.Add(startlista);
                formular.Listor.Add(epostlista);

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
