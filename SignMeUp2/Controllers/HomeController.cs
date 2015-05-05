using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.DataModel;
using SignMeUp2.Models;
using SignMeUp2.Helpers;

namespace SignMeUp2.Controllers
{
    public class HomeController : RegBaseController
    {
        private SignMeUpDataModel db = new SignMeUpDataModel();

        // GET: Home
        public ActionResult Index()
        {
            return View(db.Evenemang.ToList());
        }

        //// GET: Home
        //public ActionResult RegistrationDone(int? id)
        //{
        //    var reg = db.Registreringar.Include("Evenemang.Organisation").SingleOrDefault(r => r.ID == id);

        //    if (reg != null)
        //    {
        //        return View(reg);
        //    }

        //    return ShowError("Ingen anmälan med id: " + id + " kunde hittas. Var god gör om din anmälan.");
        //}

        /// <summary>
        /// Visa startlista
        /// </summary>
        /// <param name="id">Evenemangs ID</param>
        /// <returns></returns>
        public ActionResult Startlista(int? id)
        {
            if (id == null)
                return ShowError("Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.", false);

            var evenemang = db.Evenemang.Where(ev => ev.Id == id).FirstOrDefault();

            var evenemangResult = EvenemangHelper.EvaluateEvenemang(evenemang);

            if (evenemangResult == EvenemangHelper.EvenemangValidationResult.DoesNotExist)
            {
                return ShowError("Evenemang med id " + id.Value + " är antingen borttaget ur databasen eller felaktigt angivet.", false);
            }

            var regs = db.Registreringar.Where(reg => reg.Evenemang_Id == id.Value).ToList();
            var banor = db.Banor.ToList();
            var klasser = db.Klasser.ToList();

            var startlista = StartlistaViewModel.GetStartlist(regs, evenemang.Namn, banor, klasser);

            return View("Startlista", startlista);
        }
    }
}