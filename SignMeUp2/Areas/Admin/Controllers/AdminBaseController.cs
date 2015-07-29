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
using System.IO;

namespace SignMeUp2.Areas.Admin.Controllers
{
    public abstract class AdminBaseController : Controller
    {
        protected SignMeUpDataModel db;

        protected SignMeUpService SMU;

        protected readonly ILog log;

        protected abstract string GetEntitetsNamn();

        protected void SetViewBag(Evenemang evenemang)
        {
            ViewBag.Entitet = GetEntitetsNamn();

            if (evenemang != null)
            {
                ViewBag.Evenemang = evenemang;
                ViewBag.EvenemangsId = evenemang.Id;
            }
        }

        protected void SetViewBag(int? evenemangsId)
        {
            ViewBag.Entitet = GetEntitetsNamn();

            if (evenemangsId != null)
            {
                ViewBag.Evenemang = db.Evenemang.FirstOrDefault(e => e.Id == evenemangsId.Value);
                ViewBag.EvenemangsId = evenemangsId;
            }
        }

        protected bool IsUserAdmin
        {
            get { return User.IsInRole("admin"); }
        }

        public AdminBaseController()
        {
            log = LogManager.GetLogger(GetType());
            SMU = new SignMeUpService();
            db = SMU.Db;
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

        protected bool SkickaFaktura(Registreringar reg)
        {
            var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            var link = appUrl + "signmeup/faktura/" + reg.Id;
            SendMail.SendRegistration(RenderRazorViewToString("SignMeUp", "Faktura", reg), appUrl, link, reg);
            log.Debug("Skickat epost med faktura till lagnamn: " + reg.Lagnamn);
            return true;
        }

        protected string RenderRazorViewToString(string controllername, string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}