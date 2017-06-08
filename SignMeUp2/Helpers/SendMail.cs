using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using log4net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignMeUp2.Helpers
{
    public class SendMail
    {
        public static void SendRegistration(string message, string hostAddress, string regLink, string fakturaLink, Registrering reg)
        {
            try
            {
                message = message.Replace("href=\"/", string.Format("href=\"{0}", hostAddress));
                message = message.Replace("src=\"/", string.Format("src=\"{0}", hostAddress));
                message = message.Replace("Admin/SignMeUp/Faktura", "SignMeUp/Faktura");
                message = message.Replace("<html>", string.Format("<html>Ser meddelandet konstigt ut? Öppna följande adress i en webbläsare: {0}<br/><br/>", regLink));

                Mail mail = new Mail();
                mail.From = new Email(reg.Formular.Evenemang.Organisation.Epost, reg.Formular.Evenemang.Organisation.Namn);
                mail.AddContent(new Content("text/plain", "Följ länken för en bekräftelse på din anmälan: " + regLink));
                mail.AddContent(new Content("text/html", message));
                mail.Subject = string.Format("Bekräftelse anmälan till " + reg.Formular.Evenemang.Namn);
                var personalization = AddToAddresses(new Personalization(), reg.Svar);
                personalization.AddBcc(mail.From); // Skicka kopia till arrangör
                mail.AddPersonalization(personalization);

                Task.Run(() => SendMailv3(mail));
                //SendMailv3(mail).Wait();
            }
            catch (Exception exc)
            {
                ILog log = LogManager.GetLogger("SendMail");
                log.Error("Fel vid skickande av mail.", exc);
            }
        }

        private static Personalization AddToAddresses(Personalization pers, ICollection<FaltSvar> svarslista)
        {
            foreach (var svar in svarslista)
            {   
                if (svar.Falt.Typ == FaltTyp.epost_falt)
                {
                    pers.AddTo(new Email(svar.Varde));
                }

            }

            return pers;
        }

        private static IEnumerable<String> GetEmailAddresses(ICollection<FaltSvar> svarslista)
        {
            var newCollection = new List<String>();

            foreach(var svar in svarslista)
            {
                if (svar.Falt.Typ == FaltTyp.epost_falt)
                {
                    newCollection.Add(svar.Varde);
                }
            }

            return newCollection;
        }

        public static void SkickaFaktura(string message, string hostAddress, string link, FakturaVM fakturaVm)
        {
            try
            {
                message = message.Replace("href=\"/", string.Format("href=\"{0}", hostAddress));
                message = message.Replace("src=\"/", string.Format("src=\"{0}", hostAddress));
                message = message.Replace("<html>", string.Format("<html>Ser meddelandet konstigt ut? Öppna följande adress i en webbläsare: {0}<br/><br/>", link));

                Mail mail = new Mail();
                var arrEpost = new Email(fakturaVm.Arrangor.Epost, fakturaVm.Arrangor.Namn);
                mail.From = arrEpost;
                mail.AddContent(new Content("text/plain", "Följ länken för faktura på din anmälan: " + link));
                mail.AddContent(new Content("text/html", message));
                mail.Subject = string.Format("Faktura för anmälan till " + fakturaVm.Evenemangsnamn);
                var personalization = new Personalization();
                personalization.AddTo(new Email(fakturaVm.Fakturaadress.Epost));
                personalization.AddCc(arrEpost);
                mail.AddPersonalization(personalization);

                Task.Run(() => SendMailv3(mail));
                //SendMailv3(mail).Wait();
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
                Mail mail = new Mail();
                mail.AddContent(new Content("text/html", errorMessage));
                mail.Subject = string.Format("Fel i SignMeUp2 ({0}) {1}", host, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                var personalization = new Personalization();
                personalization.AddTo(new Email(ConfigurationManager.AppSettings["AdminEmail"]));
                mail.AddPersonalization(personalization);

                Task.Run(() => SendMailv3(mail));
                //SendMailv3(mail).Wait();
            }
            catch (Exception exc)
            {
                log.Error("Fel vid skickande av mail.", exc);
            }
        }

        public static async void SendMailv3(Mail mail)
        {
            ILog log = LogManager.GetLogger("SendMail");
            string apiKey = ConfigurationManager.AppSettings["SendGridAPI_key"];

            if (string.IsNullOrEmpty(apiKey)) {
                apiKey = Environment.GetEnvironmentVariable("APPSETTING_SendGridAPI_key");
                log.Debug("apiKey empty, on Azure? APPSETTING_SendGridAPI_key: " + apiKey);
            }

            log.Debug("SendGrid APIKey: " + apiKey);

            var sg = new SendGridAPIClient(apiKey);

            dynamic response = null;

            try
            {
                response = await sg.client.mail.send.post(requestBody: mail.Get());
            } catch (Exception exc)
            {
                log.Error("Ett fel inträffade vid skickande av mail till SendGrid", exc);
            }

            log.Debug(string.Format("Resultat skickat mail: ststusCode: {0}, responseBody: {1}, headers: {2}", response.StatusCode, response.Body.ReadAsStringAsync().Result, response.Headers.ToString()));
        }
    }
}