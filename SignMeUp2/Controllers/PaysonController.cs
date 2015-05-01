using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SignMeUp2.Models;
using SignMeUp2.Helpers;
using SignMeUp2.DataModel;
using PaysonIntegration;
using PaysonIntegration.Data;
using PaysonIntegration.Utils;

namespace SignMeUp2.Controllers
{
    public class PaysonController : RegBaseController
    {
        private const string ApplicationId = "SignMeUp2";

        // GET: Payson
        public ActionResult Index()
        {
            var registrering = (Registreringar)TempData["reg"];

            if (registrering == null)
            {   
                return RedirectToAction("Index", "SignMeUp");
            }

            var paysonViewModel = new PaysonViewModel { Registrering = registrering };
            TempData[PaysonViewModel.PAYSON_VM] = paysonViewModel;

            var paysonKontaktinfo = new PaysonKontaktViewModel
            {
                SenderEmail = registrering.Epost,
                SenderFirstName = registrering.Deltagare.FirstOrDefault().Förnamn,
                SenderLastName = registrering.Deltagare.FirstOrDefault().Efternamn
            };

            return View(paysonKontaktinfo);
        }

        [HttpPost]
        public ActionResult Index(PaysonKontaktViewModel kontaktInfo, string prev)
        {
            if (!string.IsNullOrEmpty(prev))
            {
                return RedirectToAction("BekraftaRegistrering", "SignMeUp");
            }

            if (ModelState.IsValid)
            {
                var paysonViewModel = (PaysonViewModel)TempData[PaysonViewModel.PAYSON_VM];

                if (paysonViewModel == null)
                {
                    return RedirectToAction("Index", "SignMeUp");
                }

                paysonViewModel.Kontaktinformation = kontaktInfo;

                try
                {
                    if (paysonViewModel == null || paysonViewModel.Registrering == null)
                    {
                        return ShowError("Missing data, checkout or checkout.Registrering is null at Pay()");
                    }

                    log.Debug("Payment: Lagnamn: " + paysonViewModel.Registrering.Lagnamn);

                    SaveNewRegistration(paysonViewModel.Registrering);
                    FillRegistrering(paysonViewModel.Registrering);

                    paysonViewModel.RegId = paysonViewModel.Registrering.ID;

                    // We remove port info to help when the site is behind a load balancer/firewall that does port rewrites.
                    var scheme = Request.Url.Scheme;
                    var host = Request.Url.Host;
                    //var oldPort = Request.Url.Port.ToString();
                    var returnUrl = Url.Action("Returned", "Payson", new RouteValueDictionary(), scheme, host) + "?regId=" + paysonViewModel.RegId;

                    var cancelUrl = Url.Action("Cancelled", "Payson", new RouteValueDictionary(), scheme, host) + "?regId=" + paysonViewModel.RegId;

                    var sender = new Sender(paysonViewModel.Kontaktinformation.SenderEmail);
                    sender.FirstName = paysonViewModel.Kontaktinformation.SenderFirstName;
                    sender.LastName = paysonViewModel.Kontaktinformation.SenderLastName;

                    var totalAmount = Avgift.Kalk(paysonViewModel.Registrering);

                    var receiver = new Receiver(PaysonViewModel.PaysonRecieverEmail, totalAmount);
                    receiver.FirstName = PaysonViewModel.PaysonRecieverFirstName;
                    receiver.LastName = PaysonViewModel.PaysonRecieverLastName;
                    receiver.SetPrimaryReceiver(true);

                    var evenemang = db.Evenemang.Find(paysonViewModel.Registrering.Evenemang_Id);
                    var payData = new PayData(returnUrl, cancelUrl, evenemang.Namn + " - " + paysonViewModel.Registrering.Lagnamn, sender, new List<Receiver> { receiver });

                    // Set IPN callback URL
                    // When the shop is hosted by Payson the IPN scheme must be http and not https
                    var ipnNotificationUrl = Url.Action("IPN", "Payson", new RouteValueDictionary(), scheme, host) + "?regId=" + paysonViewModel.RegId;
                    payData.SetIpnNotificationUrl(ipnNotificationUrl);

                    payData.SetFundingConstraints(new List<FundingConstraint> { FundingConstraint.Bank, FundingConstraint.CreditCard });
                    payData.SetTrackingId(paysonViewModel.Registrering.ID.ToString());

                    var orderItems = new List<PaysonIntegration.Utils.OrderItem>();
                    var reg = paysonViewModel.Registrering;
                    // Lägg in värden på kvitto
                    var oi1 = new PaysonIntegration.Utils.OrderItem("Utmaningen " + DateTime.Now.Year + ", bana " + reg.Banor.Namn);
                    oi1.SetOptionalParameters("st", 1, reg.Banor.Avgift, 0);
                    orderItems.Add(oi1);
                    if (reg.Kanoter.Avgift != 0)
                    {
                        var oi2 = new PaysonIntegration.Utils.OrderItem("Kanot, " + reg.Kanoter.Namn);
                        oi2.SetOptionalParameters("st", 1, (decimal)reg.Kanoter.Avgift, 0);
                        orderItems.Add(oi2);
                    }
                    if (reg.Forseningsavgift != 0)
                    {
                        var oi3 = new PaysonIntegration.Utils.OrderItem("Avgift för sen anmälan");
                        oi3.SetOptionalParameters("st", 1, (decimal)reg.Forseningsavgift, 0);
                        orderItems.Add(oi3);
                    }
                    if (reg.Rabatter != 0)
                    {
                        var oi4 = new PaysonIntegration.Utils.OrderItem("Rabatter");
                        oi4.SetOptionalParameters("st", 1, -(decimal)reg.Rabatter, 0);
                        orderItems.Add(oi4);
                    }
                    payData.SetOrderItems(orderItems);

                    // TODO hämta från organisation
                    var api = new PaysonApi("17224", "e656e666-3585-4453-ad39-f0ec39fa15fc", ApplicationId, false);

                    //var api = new PaysonApi(PaysonViewModel.PaysonUserId, PaysonViewModel.PaysonUserKey, ApplicationId, false);
#if DEBUG
                    api = new PaysonApi("4", "2acab30d-fe50-426f-90d7-8c60a7eb31d4", ApplicationId, true);
#endif

                    var response = api.MakePayRequest(payData);

                    if (response.Success)
                    {
                        paysonViewModel.Token = response.Token;
                        paysonViewModel.Registrering.PaysonToken = response.Token;
                        SaveChanges(paysonViewModel.Registrering);

                        var forwardUrl = api.GetForwardPayUrl(response.Token);

                        Session[PaysonViewModel.PAYSON_VM] = paysonViewModel;

                        return Redirect(forwardUrl);
                    }

                    Session[PaysonViewModel.PAYSON_VM] = paysonViewModel;

                    return ShowPaymentError("Error when sending payment to payson.", response.NvpContent, paysonViewModel.Registrering);
                }
                catch (Exception exception)
                {
                    log.Error("Exception in Index.", exception);
                    return ShowError("Oväntat fel vid betalning. Var god försök igen.", exception);
                }
            }

            return View(kontaktInfo);
        }

        /// <summary>
        /// Metod som kallas när betalningen är klar
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        public ActionResult Returned(string regId)
        {
            int registrationId = -1;

            log.Debug("Returned");

            if (!string.IsNullOrEmpty(regId) && !int.TryParse(regId, out registrationId))
            {
                ShowError("regId not in querystring at checkout returned.");
            }

            var registration = db.Registreringar.FirstOrDefault(regg => regg.ID == registrationId);

            if (registration == null)
            {
                ShowError("No registration found in db with id: " + registrationId + " in checkout returned.");
            }

            log.Debug("Returned. Lagnamn: " + registration.Lagnamn);

            // If no payment message has been sent (IPN)
            if (!registration.HarBetalt)
            {
                var api = new PaysonApi("17224", "e656e666-3585-4453-ad39-f0ec39fa15fc", ApplicationId, false);
#if DEBUG
                api = new PaysonApi("4", "2acab30d-fe50-426f-90d7-8c60a7eb31d4", ApplicationId, true);
#endif
                var response = api.MakePaymentDetailsRequest(new PaymentDetailsData(registration.PaysonToken));

                if (response.Success && (response.PaymentDetails.PaymentStatus == PaymentStatus.Completed ||
                                        response.PaymentDetails.PaymentStatus == PaymentStatus.Pending))
                {
                    if (!registration.HarBetalt)
                    {
                        SetAsPaid(registration);
                    }
                }
                else
                {
                    log.Warn("Deleting temp-registration with id: " + registrationId);
                    // Remove the temporary registration
                    DeleteRegistrering(registrationId);

                    return ShowPaymentError("Error when payment returned.", response.NvpContent, registration);
                }
            }

            return RedirectToAction("Redirect", "Home");
        }

        public ActionResult Cancelled(string regId)
        {
            int registrationId = 0;
            if (string.IsNullOrEmpty(regId) || int.TryParse(regId, out registrationId))
            {
                ShowError("Kunde inte återskapa dina uppgifter.");
            }

            var registrering = db.Registreringar.Include("Banor").Include("Evenemang").Include("Kanoter").Include("Klasser").SingleOrDefault(r => r.ID == registrationId);

            if (registrering != null)
            {
                db.Registreringar.Remove(registrering);
                log.Debug("Removed registrering with id: " + regId);
                registrering.Adress = "Mamma mia...";
                TempData["reg"] = registrering;
            }

            return RedirectToAction("Index");
        }

        public ActionResult IPN(string regId)
        {
            log.Debug("IPN regId: " + regId);

            int regIdInt = -1;
            int.TryParse(regId, out regIdInt);

            var registration = db.Registreringar.FirstOrDefault(regg => regg.ID == regIdInt);

            if (registration != null)
            {
                Request.InputStream.Position = 0;
                var content = new StreamReader(Request.InputStream).ReadToEnd();

                var api = new PaysonApi("17224", "e656e666-3585-4453-ad39-f0ec39fa15fc", ApplicationId, false);
                var response = api.MakeValidateIpnContentRequest(content);
                var statusText = response.ProcessedIpnMessage.PaymentStatus.HasValue
                                    ? response.ProcessedIpnMessage.PaymentStatus.ToString()
                                    : "N/A";
                var status = response.ProcessedIpnMessage.PaymentStatus;

                log.Debug("IPN message content: " + response.Content);
                log.Debug("IPN raw response: " + content);

                if (status == PaymentStatus.Completed || status == PaymentStatus.Completed)
                {
                    log.Debug("IPN message, status: " + statusText + ". regId: " + regId + " success: " + response.Success);

                    if (!registration.HarBetalt)
                    {
                        SetAsPaid(registration);
                    }
                }
                else
                {
                    log.Debug("IPN message for non complete transaction. regId: " + regId + ". Status: " + statusText);
                }
            }
            else
            {
                log.Error("Got IPN with wrong regId as query parameter: " + regId);
                SendMail.SendErrorMessage("Got IPN with wrong regId as query parameter: " + regId);
            }

            return new EmptyResult();
        }

        public ActionResult ShowPaymentError(string logMessage, IDictionary<string, string> response, Registreringar registration)
        {
            var str = new StringBuilder();
            str.Append(logMessage);
            str.Append("\n");

            foreach (KeyValuePair<string, string> error in response)
            {
                str.Append(error.Key + ": " + error.Value);
                str.Append("\n");
            }
            log.Error(str.ToString());

            try
            {
                SendMail.SendErrorMessage(str.ToString());
            }
            catch (Exception exception)
            {
                log.Error("Erro when sending error message.", exception);
            }

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

            TempData["reg"] = registration;

            return RedirectToAction("Index");
        }
    }
}
