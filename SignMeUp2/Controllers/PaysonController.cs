using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SignMeUp2.ViewModels;
using SignMeUp2.Helpers;
using SignMeUp2.Data;
using PaysonIntegration;
using PaysonIntegration.Data;
using PaysonIntegration.Utils;

namespace SignMeUp2.Controllers
{
    public class PaysonController : BaseController
    {
        private const string ApplicationId = "SignMeUp2";
        
        protected override string GetEntitetsNamn()
        {
            return string.Empty;
        }

        // GET: Payson
        public ActionResult Index()
        {
            var SUPVM = Session["VM"] as SignMeUpVM;
            // TODO, hantera Cancel från Payson
            // Om ViewModel finns i Session kommmer vi från Cancelled och ska visa
            // felmeddelande
            if (SUPVM == null)
            {
                return ShowError(log, "Oväntat fel inträffade. Var god försök senare.", true);
                //return View(SUPVM.Kontaktinformation);
            }

            ViewBag.ev = SUPVM.EvenemangsNamn;

            LogDebug(log, "Paysonbetalning påbörjad för lag " + SUPVM.GetFaltvarde("Lagnamn"));

            var paysonKontaktinfo = new PaysonKontaktViewModel
            {
                SenderEmail = SUPVM.GetFaltvarde("Epost"),
                SenderFirstName = SUPVM.GetFaltvarde("Förnamn 1"),
                SenderLastName = SUPVM.GetFaltvarde("Efternamn 1")
            };
            SUPVM.Kontaktinformation = paysonKontaktinfo;

            Session["VM"] = SUPVM;

            return View(paysonKontaktinfo);
        }

        [HttpPost]
        public ActionResult Index(PaysonKontaktViewModel kontaktInfo, string prev)
        {
            if (!string.IsNullOrEmpty(prev))
            {
                return RedirectToAction("BekraftaRegistrering", "SignMeUp");
            }

            var SUPVM = Session["VM"] as SignMeUpVM;

            if (ModelState.IsValid)
            {
                try
                {
                    if (SUPVM == null)
                    {
                        return ShowError(log, "Oväntat fel. Var god försök senare", true, new Exception("Ingen paysonViewModel i Session."));
                    }

                    SUPVM.Kontaktinformation = kontaktInfo;

                    LogDebug(log, string.Format("Payment: Lagnamn: {0}", SUPVM.GetFaltvarde("Lagnamn")));

                    var evenemang = smuService.HamtaEvenemang(SUPVM.EvenemangsId);
                    var org = evenemang.Organisation;

                    // Spara temporärt i databasen
                    var reg = smuService.Spara(SUPVM);
                    SUPVM.RegistreringsId = reg.Id;

                    PayData payData = SkapaPaysonPayData(SUPVM, org);

                    var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);

                    var response = api.MakePayRequest(payData);

                    if (response.Success)
                    {
                        SUPVM.PaysonToken = response.Token;
                        reg.PaysonToken = response.Token;

                        smuService.UpdateraRegistrering(reg);

                        var forwardUrl = api.GetForwardPayUrl(response.Token);

                        Session["VM"] = SUPVM;

                        return Redirect(forwardUrl);
                    }

                    Session["VM"] = SUPVM;

                    return ShowPaymentError("Error when sending payment to payson.", response.NvpContent, SUPVM.EvenemangsId);
                }
                catch (Exception exception)
                {
                    var exc = new Exception("Ett fel inträffade i PaysonController Index metod.", exception);
                    LogError(log, "Exception in Index.", exception);
                    return ShowError(log, "Oväntat fel vid betalning. Var god försök igen.", true, exc);
                }
            }
            
            Session["VM"] = SUPVM;
            ViewBag.ev = SUPVM.EvenemangsNamn;
            return View(kontaktInfo);
        }

        private PayData SkapaPaysonPayData(SignMeUpVM SUPVM, Organisation org)
        {
            // We remove port info to help when the site is behind a load balancer/firewall that does port rewrites.
            var scheme = Request.Url.Scheme;
            var host = Request.Url.Host;

            var returnUrl = Url.Action("Returned", "Payson", new RouteValueDictionary(), scheme, host) + "/" + SUPVM.RegistreringsId;

            var cancelUrl = Url.Action("Cancelled", "Payson", new RouteValueDictionary(), scheme, host) + "/" + SUPVM.RegistreringsId;

            var sender = new Sender(SUPVM.Kontaktinformation.SenderEmail);
            sender.FirstName = SUPVM.Kontaktinformation.SenderFirstName;
            sender.LastName = SUPVM.Kontaktinformation.SenderLastName;

            var receiver = new Receiver(org.Epost, SUPVM.AttBetala);
            receiver.FirstName = org.Namn;
            receiver.SetPrimaryReceiver(true);

            var payData = new PayData(returnUrl,
                cancelUrl,
                smuService.HamtaEvenemang(SUPVM.EvenemangsId).Namn + " - " + SUPVM.GetFaltvarde("Lagnamn"),
                sender,
                new List<Receiver> { receiver });

            // Set IPN callback URL
            // When the shop is hosted by Payson the IPN scheme must be http and not https
            var ipnNotificationUrl = Url.Action("IPN", "Payson", new RouteValueDictionary(), scheme, host) + "/" + SUPVM.RegistreringsId;
            payData.SetIpnNotificationUrl(ipnNotificationUrl);

            payData.SetFundingConstraints(new List<FundingConstraint> { FundingConstraint.Bank, FundingConstraint.CreditCard });
            payData.SetTrackingId(SUPVM.RegistreringsId.ToString());

            // Skapa poster för betalning
            var orderItems = new List<PaysonIntegration.Utils.OrderItem>();
            foreach (ValViewModel post in SUPVM.Betalnignsposter)
            {
                var oi = new PaysonIntegration.Utils.OrderItem(post.TypNamn + ": " + post.Namn);
                oi.SetOptionalParameters("st", 1, post.Avgift, 0);
                orderItems.Add(oi);
            }

            payData.SetOrderItems(orderItems);

            return payData;
        }

        /// <summary>
        /// Metod som kallas när betalningen är klar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Returned(int? id)
        {
            LogDebug(log, string.Format("Returned id: {0}", id));

            if (!id.HasValue)
            {
                LogError(log, string.Format("Felaktigt angivet regId: {0}", id));
                return ShowError(log, "Ett fel inträffade när betalningen slutfördes. Kontrollera i startlistan om er registrering genomförts", true, new Exception("Felaktigt angivet regId: " + id));
            }

            var registration = smuService.GetRegistrering(id.Value, true);

            if (registration == null)
            {
                LogError(log, string.Format("Ingen registration hittad med id: {0} i Returned.", id));
                return ShowError(log, "Ett fel inträffade när betalningen slutfördes. Kontrollera i startlistan om er registrering genomförts", true, new Exception("Ingen registration hittad med  id: " + id + " i Returned."));
            }

            LogDebug(log, string.Format("Returned. Lagnamn: {0}", registration.Lagnamn));

            // If no payment message has been sent (IPN)
            if (!registration.HarBetalt)
            {
                var org = smuService.HamtaOrganisation(registration.Evenemang.OrganisationsId);

                var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);

                var response = api.MakePaymentDetailsRequest(new PaymentDetailsData(registration.PaysonToken));

                if (response.Success && (response.PaymentDetails.PaymentStatus == PaymentStatus.Completed ||
                                        response.PaymentDetails.PaymentStatus == PaymentStatus.Pending))
                {
                    if (!registration.HarBetalt)
                    {
                        smuService.HarBetalt(registration);
                        SkickaRegMail(registration);
                        Session["VM"] = null;
                    }
                }
                else
                {
                    LogDebug(log, string.Format("Deleting temp-registration with id: {0}", id));

                    var evenemangsId = registration.EvenemangsId;

                    // Remove the temporary registration
                    smuService.TabortRegistrering(registration);

                    return ShowPaymentError("Error when payment returned.", response.NvpContent, evenemangsId.Value);
                }
            }

            ViewBag.ev = registration.Evenemang.Namn;
            return RedirectToAction("BekraftelseBetalning", "signmeup", new { id = id });
        }

        public ActionResult Cancelled(int? id)
        {
            if (!id.HasValue)
            {
                LogError(log, string.Format("Felaktigt regId: {0} i cancel.", id));
                return ShowError(log, "Kunde inte återskapa dina uppgifter.", true, new Exception("Felaktigt regId: " + id + " vid cancel."));
            }

            var registrering = smuService.GetRegistrering(id.Value, false);

            if (registrering == null)
            {
                LogError(log, "Payson betalning avbruten och ingen registrering i Session hittades.");
                return ShowError(log, "Betalningen avbröts av okänd anledning", true, new Exception("Payson betalning avbruten och ingen registrering i Session hittades."));
            }

            var org = smuService.HamtaOrganisation(registrering.Evenemang.OrganisationsId);

            var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);

            var response = api.MakePaymentDetailsRequest(new PaymentDetailsData(registrering.PaysonToken));

            var evenemangsId = registrering.EvenemangsId;

            // Ta bort temporär registrering
            smuService.TabortRegistrering(registrering);

            Session["VM"] = null;

            return ShowPaymentError("Betalningen avbruten.", response.NvpContent, evenemangsId.Value);
        }

        public ActionResult IPN(int? id)
        {
            var host = Request.Url.Host;

            if (!id.HasValue)
            {
                LogError(log, "IPN, id har inget värde");
                SendMail.SendErrorMessage("IPN, id har inget värde", host);
                return new EmptyResult();
            }

            LogDebug(log, string.Format("IPN id: {0}", id));

            try
            {
                var registrering = smuService.GetRegistrering(id.Value, true);

                if (registrering != null)
                {
                    Request.InputStream.Position = 0;
                    var content = new StreamReader(Request.InputStream).ReadToEnd();

                    var org = smuService.HamtaOrganisation(registrering.Evenemang.OrganisationsId);
                    var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);

                    var response = api.MakeValidateIpnContentRequest(content);
                    var statusText = response.ProcessedIpnMessage.PaymentStatus.HasValue
                                        ? response.ProcessedIpnMessage.PaymentStatus.ToString()
                                        : "N/A";
                    var status = response.ProcessedIpnMessage.PaymentStatus;

                    LogDebug(log, string.Format("IPN message, status: {0}. regId: {1} success: {2}",
                        statusText, id, response.Success));

                    if (status == PaymentStatus.Completed)
                    {
                        if (!registrering.HarBetalt)
                        {
                            smuService.HarBetalt(registrering);
                            SkickaRegMail(registrering);
                            Session["VM"] = null;
                        }
                        else
                        {
                            LogDebug(log, "Registreringen var redan markerad som betald. Skickar inget meddelande.");
                        }
                    }
                    else
                    {
                        SendMail.SendErrorMessage("IPN message for non complete transaction. regId: " + id + ". Status: " + statusText, host);
                        LogDebug(log, string.Format("IPN message for non complete transaction. regId: {0}. Status: {1}", id, statusText));
                    }
                }
                else
                {
                    LogError(log, string.Format("Got IPN with wrong regId as query parameter: {0}", id));
                    SendMail.SendErrorMessage("Got IPN with wrong regId as query parameter: " + id, host);
                }
            }
            catch (Exception exc)
            {
                LogError(log, "Ett fel inträffade i IPN metoden.", exc);
                SendMail.SendErrorMessage(string.Format("Ett fel inträffade i IPN metoden. Exception: {0}", exc.ToString()), host);
            }

            return new EmptyResult();
        }

        public ActionResult ShowPaymentError(string logMessage, IDictionary<string, string> response, int evenemangsId)
        {
            var str = new StringBuilder();
            str.Append(logMessage);
            str.Append("\n");

            foreach (KeyValuePair<string, string> error in response)
            {
                str.Append(error.Key + ": " + error.Value);
                str.Append("\n");
            }
            LogError(log, str.ToString());

            if (response.ContainsKey("errorList.error(0).message"))
            {
                TempData["PaymentErrorMessage"] = response["errorList.error(0).message"];

                if (response.ContainsKey("errorList.error(0).parameter"))
                {
                    TempData["PaymentErrorParameter"] = response["errorList.error(0).parameter"];
                }
            }
            else
            {
                TempData["PaymentErrorMessage"] = "Betalningen avbröts av okänd anledning.";
                TempData["PaymentErrorParameter"] = "Okänd";
            }

            return RedirectToAction("Index", "SignMeUp", new { id = evenemangsId });
        }
    }
}
