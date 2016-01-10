//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using SignMeUp2.Data;
//using SignMeUp2.Controllers;

//namespace SignMeUp2.Areas.Admin.Controllers
//{
//    [Authorize]
//    public class BanorController : BaseController
//    {
//        private static string _entity = "Banor";
//        protected override string GetEntitetsNamn()
//        {
//            return _entity;
//        }


//        // GET: Admin/Banor
//        public ActionResult Index(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            SetViewBag(id);

//            var banor = db.Banor.Include(b => b.Evenemang).Where(b => b.EvenemangsId == id.Value);

//            return View(banor.ToList());
//        }

//        // GET: Admin/Banor/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Banor banor = db.Banor.Find(id);
//            if (banor == null)
//            {
//                return HttpNotFound();
//            }

//            SetViewBag(banor.EvenemangsId);

//            return View(banor);
//        }

//        // GET: Admin/Banor/Create
//        public ActionResult Create(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            SetViewBag(id);

//            return View();
//        }

//        // POST: Admin/Banor/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "ID,Namn,Avgift,AntalDeltagare")] Banor bana, int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            ValidateModel(bana);

//            if (ModelState.IsValid)
//            {
//                bana.EvenemangsId = id.Value;
//                db.Banor.Add(bana);
//                db.SaveChanges();
//                return RedirectToAction("Index", new { id = id });
//            }

//            SetViewBag(id);
//            return View(bana);
//        }

//        // GET: Admin/Banor/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Banor banor = db.Banor.Find(id);
//            if (banor == null)
//            {
//                return HttpNotFound();
//            }

//            SetViewBag(banor.EvenemangsId);

//            return View(banor);
//        }

//        // POST: Admin/Banor/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "ID,Namn,Avgift,AntalDeltagare,EvenemangsId")] Banor bana)
//        {
//            SetViewBag(bana.EvenemangsId);

//            if (ModelState.IsValid)
//            {
//                db.Entry(bana).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index", new { id = bana.EvenemangsId });
//            }

//            return View(bana);
//        }

//        // GET: Admin/Banor/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Banor banor = db.Banor.Find(id);
//            if (banor == null)
//            {
//                return HttpNotFound();
//            }

//            SetViewBag(banor.EvenemangsId);

//            return View(banor);
//        }

//        // POST: Admin/Banor/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Banor bana = db.Banor.Find(id);
//            var evId = bana.EvenemangsId;
//            try
//            {
//                db.Banor.Remove(bana);
//                db.SaveChanges();
//                return RedirectToAction("Index", new { id = evId });
//            }
//            catch (Exception)
//            {
//                SetViewBag(evId);
//                ViewBag.Error = "Kunde inte ta bort denna bana. Det kan vara så att den används i en registrering.";
//                return View(bana);
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
