using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.DataModel;
using SignMeUp2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;

namespace SignMeUp2.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        protected SignMeUpDataModel db = new SignMeUpDataModel();

        protected ApplicationUser GetUser()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return userManager.FindById(User.Identity.GetUserId());
        }
    }
}