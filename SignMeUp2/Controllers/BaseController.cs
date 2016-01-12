using System;
using System.IO;
using System.Web.Mvc;
using log4net;
using SignMeUp2.Helpers;
using SignMeUp2.Data;
using SignMeUp2.Services;
using SignMeUp2.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace SignMeUp2.Controllers
{
    public abstract class BaseController : Controller
    {
        protected SignMeUpService smuService;
        protected SignMeUpDataModel db;
        protected readonly ILog log;

        public BaseController()
        {   
            log = LogManager.GetLogger(GetType());
            smuService = new SignMeUpService();
            db = smuService.Db;
        }

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
                ViewBag.Evenemang = smuService.HamtaEvenemang(evenemangsId.Value);
                ViewBag.EvenemangsId = evenemangsId;
            }
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

        protected void SkickaRegMail(Registrering reg)
        {
            try
            {
                //smuService.FillRegistrering(reg);
                var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                var link = appUrl + "signmeup/bekraftelsebetalning/" + reg.Id;
                SendMail.SendRegistration(RenderRazorViewToString("BekraftelseMail", reg), appUrl, link, reg);
                log.Debug("Skickat epost till ?"); // lagnamn: " + reg.Lagnamn);
            }
            catch (Exception exc)
            {
                log.Error("Unable to send confirmation mail.", exc);
            }
        }

        protected FakturaVM SkapaFakturaVM(Registrering reg)
        {
            var evenemang = smuService.HamtaEvenemang(reg.Formular.EvenemangsId.Value);
            var arrangor = smuService.HamtaOrganisation(evenemang.OrganisationsId);

            if (reg.Invoice == null)
                throw new Exception("Registreringen har ingen faktureringsadress.");

            return new FakturaVM
            {
                Registrering = reg,
                Arrangor = arrangor,
                Evenemangsnamn = evenemang.Namn,
                BetalaSenast = evenemang.FakturaBetaldSenast,
                Fakturaadress = ClassMapper.MappTillInvoiceVM(reg.Invoice),
                Betalningsmetoder = ClassMapper.MappaTillBetalningsmetoderVM(arrangor.Betalningsmetoder)
            };
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

        protected bool SkickaFaktura(Registrering reg)
        {
            var fakturaVm = SkapaFakturaVM(reg);
            try
            {
                var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                var link = appUrl + "signmeup/faktura/" + reg.Id;
                SendMail.SkickaFaktura(RenderRazorViewToString("_faktura", fakturaVm), appUrl, link, fakturaVm);
                log.Debug("Skickat epost till ?"); // lagnamn: " + reg.Lagnamn);
                return true;
            }
            catch (Exception exc)
            {
                log.Error("Unable to send confirmation mail.", exc);
            }
            return false;
        }

        public ActionResult Faktura(int? id)
        {
            if (id == null)
                return ShowError(log, "Fel vid hämtning av registreringsinformation för faktura. Administratör är kontaktad. Var god försök senare", true);

            var reg = smuService.GetRegistrering(id.Value, true);
            var fakturaVm = SkapaFakturaVM(reg);

            ViewBag.ev = reg.Formular.Evenemang.Namn;

            LogDebug(log, "Användare hämtar faktura för betalning (GET) för " + reg.Formular.Evenemang.Namn + " och reg id: " + reg.Id);

            return View("_faktura", fakturaVm);
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

        protected bool IsUserAdmin
        {
            get { return User.IsInRole("admin"); }
        }

        protected ApplicationUser HamtaUser()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return userManager.FindById(User.Identity.GetUserId());
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

        protected Organisation HamtaOrganisation()
        {
            var user = HamtaUser();
            return db.Organisationer.Find(user.OrganisationsId);
        }
        
    }
}