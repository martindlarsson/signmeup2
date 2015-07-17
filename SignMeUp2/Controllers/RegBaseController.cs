using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using log4net;
using SignMeUp2.Helpers;
using SignMeUp2.Data;
using SignMeUp2.Services;

namespace SignMeUp2.Controllers
{
    public abstract class RegBaseController : Controller
    {
        protected SignMeUpService smuService;
        protected readonly ILog log;

        public RegBaseController()
        {   
            log = LogManager.GetLogger(GetType());
            smuService = SignMeUpService.Instance;
        }

        protected string RenderRazorViewToString(string viewName, object model)
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

        protected void SkickaRegMail(Registreringar reg)
        {
            try
            {
                smuService.FillRegistrering(reg);
                var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                var link = appUrl + "signmeup/bekraftelsebetalning/" + reg.Id;
                SendMail.SendRegistration(RenderRazorViewToString("BekraftelseMail", reg), appUrl, link, reg);
                log.Debug("Skickat epost till lagnamn: " + reg.Lagnamn);
            }
            catch (Exception exc)
            {
                log.Error("Unable to send confirmation mail.", exc);
            }
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
    }
}