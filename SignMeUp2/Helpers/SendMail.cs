using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using log4net;

namespace SignMeUp2.Helpers
{
    public class SendMail
    {
        public static void SendRegistration(string message, string hostAddress, string link, Registreringar reg)
        {
            try
            {
                message = message.Replace("href=\"/", string.Format("href=\"{0}", hostAddress));
                message = message.Replace("src=\"/", string.Format("src=\"{0}", hostAddress));
                message = message.Replace("<html>", string.Format("<html>Ser meddelandet konstigt ut? Öppna följande adress i en webbläsare: {0}<br/><br/>", link));

                // Create the email object first, then add the properties.
                SendGridMessage myMessage = new SendGridMessage();
                myMessage.AddTo(reg.Epost);
                myMessage.From = new MailAddress(reg.Formular.Evenemang.Organisation.Epost, reg.Formular.Evenemang.Organisation.Namn);
                myMessage.Subject = string.Format("Bekräftelse anmälan till " + reg.Formular.Evenemang.Namn);
                myMessage.Html = message;
                myMessage.Text = "Följ länken för en bekräftelse på din anmälan: " + link;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGridUser"], ConfigurationManager.AppSettings["SendGridPwd"]);

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                // You can also use the **DeliverAsync** method, which returns an awaitable task.
                transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception exc)
            {
                ILog log = LogManager.GetLogger("SendMail");
                log.Error("Fel vid skickande av mail.", exc);
            }
        }

        public static void SkickaFaktura(string message, string hostAddress, string link, FakturaVM fakturaVm)
        {
            try
            {
                message = message.Replace("href=\"/", string.Format("href=\"{0}", hostAddress));
                message = message.Replace("src=\"/", string.Format("src=\"{0}", hostAddress));
                message = message.Replace("<html>", string.Format("<html>Ser meddelandet konstigt ut? Öppna följande adress i en webbläsare: {0}<br/><br/>", link));

                var arrEpost = new MailAddress(fakturaVm.Arrangor.Epost, fakturaVm.Arrangor.Namn);
                // Create the email object first, then add the properties.
                SendGridMessage myMessage = new SendGridMessage();
                myMessage.AddTo(fakturaVm.Registrering.Epost);
                myMessage.AddCc(arrEpost);
                myMessage.From = arrEpost;
                myMessage.Subject = string.Format("Faktura för anmälan till " + fakturaVm.Evenemangsnamn);
                myMessage.Html = message;
                myMessage.Text = "Följ länken för faktura på din anmälan: " + link;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGridUser"], ConfigurationManager.AppSettings["SendGridPwd"]);

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                // You can also use the **DeliverAsync** method, which returns an awaitable task.
                transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception exc)
            {
                ILog log = LogManager.GetLogger("SendMail");
                log.Error("Fel vid skickande av mail.", exc);
            }
        }

        public static void SendErrorMessage(string errorMessage, string host)
        {
#if DEBUG
            return;
#endif

            ILog log = LogManager.GetLogger("SendMail");
            log.Debug("Skickar error-mail");

            try
            {
                // Create the email object first, then add the properties.
                SendGridMessage myMessage = new SendGridMessage();
                myMessage.AddTo(ConfigurationManager.AppSettings["AdminEmail"]);
                myMessage.From = new MailAddress(ConfigurationManager.AppSettings["SystemEmail"], ConfigurationManager.AppSettings["SystemName"]);
                myMessage.Subject = string.Format("Fel i SignMeUp2 ({0}) {1}", host, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                myMessage.Html = errorMessage;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGridUser"], ConfigurationManager.AppSettings["SendGridPwd"]);

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                // You can also use the **DeliverAsync** method, which returns an awaitable task.
                transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception exc)
            {
                log.Error("Fel vid skickande av mail.", exc);
            }
        }
    }
}