﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Models;
using SignMeUp2.Helpers;

namespace SignMeUp2.Controllers
{
    public class HomeController : BaseController
    {
        protected override string GetEntitetsNamn()
        {
            return string.Empty;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View(smuService.Db.Evenemang
                .Include("Organisation")
                .Where(e => e.RegStart <= DateTime.Now && e.RegStop > DateTime.Now)
                .ToList());
        }

        /// <summary>
        /// Visa startlista
        /// </summary>
        /// <param name="id">Evenemangs ID</param>
        /// <returns></returns>
        public ActionResult Startlista(int? id)
        {
            if (id == null)
                return ShowError(log, "Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.", false);

            var evenemang = smuService.Db.Evenemang.Find(id);

            var evenemangResult = EvenemangHelper.EvaluateEvenemang(evenemang);

            if (evenemangResult == EvenemangHelper.EvenemangValidationResult.DoesNotExist)
            {
                return ShowError(log, "Evenemang med id " + id.Value + " är antingen borttaget ur databasen eller felaktigt angivet.", false);
            }

            var regs = smuService.Db.Registreringar.Include("Deltagare").Where(reg => reg.EvenemangsId == id.Value && reg.HarBetalt).ToList();
            var banor = smuService.Db.Banor.ToList();
            var klasser = smuService.Db.Klasser.ToList();

            var startlista = StartlistaViewModel.GetStartlist(regs, evenemang.Namn, banor, klasser);

            ViewBag.ev = evenemang.Namn;
            return View("Startlista", startlista);
        }
    }
}