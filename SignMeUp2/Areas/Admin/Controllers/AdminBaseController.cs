using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using log4net;
using SignMeUp2.Services;

namespace SignMeUp2.Areas.Admin.Controllers
{
    public abstract class AdminBaseController : Controller
    {
        protected SignMeUpDataModel db;

        protected readonly ILog log;

        public AdminBaseController()
        {
            log = LogManager.GetLogger(GetType());
            db = SignMeUpService.Instance.Db; //System.Web.HttpContext.Current.Items["_EntityContext"] as SignMeUpDataModel;
        }

        protected ApplicationUser GetUser()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return userManager.FindById(User.Identity.GetUserId());
        }
    }
}