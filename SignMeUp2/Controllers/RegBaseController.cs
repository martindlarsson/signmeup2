using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using log4net;
using SignMeUp2.Helpers;
using SignMeUp2.DataModel;

namespace SignMeUp2.Controllers
{
    public abstract class RegBaseController : Controller
    {
        protected SignMeUpDataModel db = new SignMeUpDataModel();
        protected readonly ILog log;

        public RegBaseController()
        {   
            log = LogManager.GetLogger(GetType());
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

        protected void SetAsPaid(Registreringar reg)
        {
            reg.HarBetalt = true;
            UpdateraReg(reg);
            SkickaRegMail(reg);
        }

        protected void SkickaRegMail(Registreringar reg)
        {
            try
            {
                FillRegistrering(reg);
                var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                var link = appUrl + "signmeup/bekraftelsebetalning/" + reg.ID;
                SendMail.SendRegistration(RenderRazorViewToString("BekraftelseMail", reg), appUrl, link, reg);
                log.Debug("Skickat epost till lagnamn: " + reg.Lagnamn);
            }
            catch (Exception exc)
            {
                log.Error("Unable to send confirmation mail.", exc);
            }
        }

        protected void SaveNewRegistration(Registreringar reg)
        {
            reg.Banor = null;
            reg.Kanoter = null;
            reg.Klasser = null;
            
            try
            {
                db.Registreringar.Add(reg);
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    log.Error(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        log.Error(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                throw;
            }
        }

        protected void UpdateraReg(Registreringar updatedReg)
        {
            var origReg = db.Registreringar.Find(updatedReg.ID);
            db.Entry(updatedReg).CurrentValues.SetValues(origReg);
            db.SaveChanges();
        }

        protected void FillRegistrering(Registreringar reg)
        {
            reg.Kanoter = db.Kanoter.Find(reg.Kanot);
            reg.Banor = db.Banor.Find(reg.Bana);
            reg.Klasser = db.Klasser.Find(reg.Klass);
            reg.Evenemang = db.Evenemang.Find(reg.Evenemang_Id);
            reg.Evenemang.Organisation = db.Organisationer.Find(reg.Evenemang.OrganisationsId);
        }

        protected ActionResult ShowError(string logMessage, bool sendMial, Exception exception = null)
        {
            log.Error(logMessage, exception);
            try
            {
                if (exception != null)
                {
                    SendMail.SendErrorMessage(logMessage + "\n\n" + exception.Message + "\n\n" + exception.StackTrace);
                }
                else
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