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
        // GET: Admin/Admin
        public ActionResult Index()
        {
            // if role admin then show all
            return View(HamtaEvenemangForAnv());
        }
    }
}
