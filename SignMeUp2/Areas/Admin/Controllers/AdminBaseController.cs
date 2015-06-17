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
using SignMeUp2.Helpers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    public abstract class AdminBaseController : Controller
    {
        protected SignMeUpDataModel db;

        protected readonly ILog log;

        protected bool IsUserAdmin
        {
            get { return User.IsInRole("admin"); }
        }

        public AdminBaseController()
        {
            log = LogManager.GetLogger(GetType());
            db = SignMeUpService.Instance.Db;
        }

        protected ApplicationUser HamtaUser()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return userManager.FindById(User.Identity.GetUserId());
        }

        protected Organisation HamtaOrganisation()
        {
            var user = HamtaUser();
            return db.Organisationer.Find(user.OrganisationsId);
        }

        protected IList<Evenemang> HamtaEvenemangForAnv()
        {
            if (User.IsInRole("admin"))
            {
                return db.Evenemang.Include("Organisation").ToList();
            }

            var user = HamtaUser();
            var events = from e in db.Evenemang
                         where e.OrganisationsId == user.OrganisationsId
                         select e;
            return events.ToList();
        }

        protected void LogDebug(ILog logger, string message)
        {
            logger.Debug(string.Format("Session: {0}. {1}", HttpContext.Session.SessionID, message));
        }

        protected void LogError(ILog logger, string message, Exception exc = null)
        {
            if (exc != null)
                logger.Error(string.Format("Session: {0}. {1}", HttpContext.Session.SessionID, message), exc);
            else
                logger.Error(string.Format("Session: {0}. {1}", HttpContext.Session.SessionID, message));
        }

        protected ActionResult ShowError(ILog logger, string logMessage, bool sendMial, Exception exception = null)
        {
            try
            {
                var host = Request.Url.Host;

                LogError(logger, logMessage, exception);

                if (sendMial && exception != null)
                {
                    SendMail.SendErrorMessage(logMessage + "<br/><br/>Felmeddelande: " + exception.Message + "<br/><br/>StackTrace: " + exception.ToString(), host);
                }
                else if (sendMial)
                {
                    SendMail.SendErrorMessage(logMessage, host);
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format("Session: {0}. Error sending error mail", HttpContext.Session.SessionID), exc);
            }

            TempData["error"] = logMessage;

            return RedirectToAction("Index", "Error", null);
        }

        //protected Models.EvenemangsValjare HamtaEvValjare(int? valtEvenemangsId)
        //{
        //    var ev = HamtaEvenemangForAnv();
        //    var valjare = new Models.EvenemangsValjare
        //    {
        //        Evenemang = new SelectList(ev, "Id", "Namn")
        //    };
        //    if (valtEvenemangsId != null)
        //    {
        //        valjare.SelectedEvenemangsId = valtEvenemangsId.Value;
        //        var valtEvenemang = ev.FirstOrDefault(e => e.Id == valtEvenemangsId.Value);
        //        if (valtEvenemang == null)
        //            throw new Exception("Felaktigt evenemangsid använt. Detta evenemang finns inte med bland användarens valbara evenemang.");
        //        valjare.SelectedEvenemangsNamn = valtEvenemang.Namn;
        //    }
        //    return valjare;
        //}
    }
}