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

        protected void FillViewBag(int evenemangsId)
        {
            ViewBag.Bana = new SelectList(smuService.Db.Banor.Where(b => b.EvenemangsId == evenemangsId).ToList(), "ID", "Namn");
            ViewBag.Kanot = new SelectList(smuService.Db.Kanoter.Where(b => b.EvenemangsId == evenemangsId).ToList(), "ID", "Namn");
            ViewBag.Klass = new SelectList(smuService.Db.Klasser.Where(b => b.EvenemangsId == evenemangsId).ToList(), "ID", "Namn");
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

        protected ActionResult ShowError(string logMessage, bool sendMial, Exception exception = null)
        {
            log.Error(logMessage, exception);
            try
            {
                if (sendMial && exception != null)
                {   
                    SendMail.SendErrorMessage(logMessage + "<br/><br/>Felmeddelande: " + exception.Message + "<br/><br/>StackTrace: " + exception.ToString());
                }
                else if (sendMial)
                {
                    SendMail.SendErrorMessage(logMessage);
                }
            }
            catch (Exception exc)
            {
                log.Error("Error sending error mail.", exc);
            }

            TempData["error"] = logMessage;

            return RedirectToAction("Index", "Error", null);
        }
    }
}