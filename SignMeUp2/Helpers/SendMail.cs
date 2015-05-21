using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SignMeUp2.Data;

namespace SignMeUp2.Helpers
{
    public class SendMail
    {
        public static void SendRegistration(string message, string hostAddress, string link, Registreringar reg)
        {   
            message = message.Replace("href=\"/", string.Format("href=\"{0}", hostAddress));
            message = message.Replace("src=\"/", string.Format("src=\"{0}", hostAddress));
            message = message.Replace("<html>", string.Format("<html>Ser meddelandet konstigt ut? Öppna följande adress i en webbläsare: {0}<br/><br/>", link));

            // Create the email object first, then add the properties.
            SendGridMessage myMessage = new SendGridMessage();
            myMessage.AddTo(reg.Epost);
            myMessage.From = new MailAddress(reg.Evenemang.Organisation.Epost, reg.Evenemang.Organisation.Namn);
            myMessage.Subject = string.Format("Bekräftelse anmälan till " + reg.Evenemang.Namn);
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

        public static void SendErrorMessage(string errorMessage)
        {
#if DEBUG
            return;
#endif
            try
            {
                // Create the email object first, then add the properties.
                SendGridMessage myMessage = new SendGridMessage();
                myMessage.AddTo(ConfigurationManager.AppSettings["AdminEmail"]);
                myMessage.From = new MailAddress(ConfigurationManager.AppSettings["SystemEmail"], ConfigurationManager.AppSettings["SystemName"]);
                myMessage.Subject = string.Format("Fel i SignMeUp2");
                myMessage.Html = errorMessage;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGridUser"], ConfigurationManager.AppSettings["SendGridPwd"]);

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                // You can also use the **DeliverAsync** method, which returns an awaitable task.
                transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception)
            {
            }
        }
    }
}