using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : AdminBaseController
    {
        private static string _entity = "Admin";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Admin/Admin
        public ActionResult Index()
        {
            var ev = HamtaEvenemangForAnv();

            ViewBag.Evenemang = new SelectList(ev, "Id", "Namn");
            // Lägg till ngt i ViewBag som fyller dropdown för val av evenemang

            ViewBag.IsAdmin = User.IsInRole("admin");
            // if role admin then show all
            return View(ev);
        }
    }
}
