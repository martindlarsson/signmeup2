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

            log.Debug("Paysonbetalning påbörjad för lag " + registrering.Lagnamn);

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

            ViewBag.ev = registrering.Evenemang.Namn;
            return View(paysonKontaktinfo);
        }

        [HttpPost]
        public ActionResult Index(PaysonKontaktViewModel kontaktInfo, string prev)
        {
            if (!string.IsNullOrEmpty(prev))
            {
                return RedirectToAction("BekraftaRegistrering", "SignMeUp");
            }

            var paysonViewModel = (PaysonViewModel)TempData[PaysonViewModel.PAYSON_VM];

            if (ModelState.IsValid)
            {
                try
                {
                    if (paysonViewModel == null || paysonViewModel.Registrering == null)
                    {
                        return ShowError("Oväntat fel. Var god försök senare", true, new Exception("Ingen paysonViewModel i TempData."));
                    }

                    paysonViewModel.Kontaktinformation = kontaktInfo;

                    log.Debug("Payment: Lagnamn: " + paysonViewModel.Registrering.Lagnamn);

                    SaveNewRegistration(paysonViewModel.Registrering);

                    paysonViewModel.RegId = paysonViewModel.Registrering.ID;

                    // We remove port info to help when the site is behind a load balancer/firewall that does port rewrites.
                    var scheme = Request.Url.Scheme;
                    var host = Request.Url.Host;

                    var returnUrl = Url.Action("Returned", "Payson", new RouteValueDictionary(), scheme, host) + "/" + paysonViewModel.RegId;

                    var cancelUrl = Url.Action("Cancelled", "Payson", new RouteValueDictionary(), scheme, host) + "/" + paysonViewModel.RegId;

                    var sender = new Sender(paysonViewModel.Kontaktinformation.SenderEmail);
                    sender.FirstName = paysonViewModel.Kontaktinformation.SenderFirstName;
                    sender.LastName = paysonViewModel.Kontaktinformation.SenderLastName;

                    FillRegistrering(paysonViewModel.Registrering);
                    var totalAmount = Avgift.Kalk(paysonViewModel.Registrering);

                    var orgId = paysonViewModel.Registrering.Evenemang.OrganisationsId;
                    var org = db.Organisationer.Include("Betalningsmetoder").Single(o => o.ID == orgId);

                    var receiver = new Receiver(org.Epost, totalAmount);
                    receiver.FirstName = org.Namn;
                    //receiver.LastName = PaysonViewModel.PaysonRecieverLastName;
                    receiver.SetPrimaryReceiver(true);

                    var evenemang = db.Evenemang.Find(paysonViewModel.Registrering.Evenemang_Id);
                    var payData = new PayData(returnUrl, cancelUrl, evenemang.Namn + " - " + paysonViewModel.Registrering.Lagnamn, sender, new List<Receiver> { receiver });

                    // Set IPN callback URL
                    // When the shop is hosted by Payson the IPN scheme must be http and not https
                    var ipnNotificationUrl = Url.Action("IPN", "Payson", new RouteValueDictionary(), scheme, host) + "/" + paysonViewModel.RegId;
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

                    var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);
#if DEBUG
                    api = new PaysonApi("4", "2acab30d-fe50-426f-90d7-8c60a7eb31d4", ApplicationId, true);
#endif

                    var response = api.MakePayRequest(payData);

                    if (response.Success)
                    {
                        paysonViewModel.Token = response.Token;
                        paysonViewModel.Registrering.PaysonToken = response.Token;
                        UpdateraReg(paysonViewModel.Registrering);

                        var forwardUrl = api.GetForwardPayUrl(response.Token);

                        Session[PaysonViewModel.PAYSON_VM] = paysonViewModel;

                        return Redirect(forwardUrl);
                    }

                    TempData[PaysonViewModel.PAYSON_VM] = paysonViewModel;

                    return ShowPaymentError("Error when sending payment to payson.", response.NvpContent, paysonViewModel.Registrering);
                }
                catch (Exception exception)
                {
                    log.Error("Exception in Index.", exception);
                    return ShowError("Oväntat fel vid betalning. Var god försök igen.", true, exception);
                }
            }

            ViewBag.ev = paysonViewModel.Registrering.Evenemang.Namn;
            return View(kontaktInfo);
        }

        /// <summary>
        /// Metod som kallas när betalningen är klar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Returned(int? id)
        {
            log.Debug("Returned");

            if (!id.HasValue)
            {
                ShowError("Ett fel inträffade när betalningen slutfördes. Kontrollera i startlistan om er registrering genomförts", true, new Exception("Felaktigt angivet regId: " + id));
            }

            var registration = db.Registreringar.FirstOrDefault(regg => regg.ID == id);

            if (registration == null)
            {
                ShowError("Ett fel inträffade när betalningen slutfördes. Kontrollera i startlistan om er registrering genomförts", true, new Exception("Ingen registration hittad med  id: " + id + " i Returned."));
            }

            log.Debug("Returned. Lagnamn: " + registration.Lagnamn);

            // If no payment message has been sent (IPN)
            if (!registration.HarBetalt)
            {
                var org = db.Organisationer.Include("Betalningsmetoder").Single(o => o.ID == registration.Evenemang.OrganisationsId);

                var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);
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
                        TempData[PaysonViewModel.PAYSON_VM] = null;
                    }
                }
                else
                {
                    log.Warn("Deleting temp-registration with id: " + id);
                    // Remove the temporary registration
                    db.Registreringar.Remove(registration);
                    db.SaveChanges();

                    return ShowPaymentError("Error when payment returned.", response.NvpContent, registration);
                }
            }

            ViewBag.ev = registration.Evenemang.Namn;
            return RedirectToAction("BekraftelseBetalning", "signmeup", new { id = id });
        }

        public ActionResult Cancelled(int? id)
        {
            if (!id.HasValue)
            {
                return ShowError("Kunde inte återskapa dina uppgifter.", true, new Exception("Felaktigt regId: " + id + " vid cancel."));
            }

            var registrering = db.Registreringar.Find(id);

            if (registrering == null)
            {
                return ShowError("Betalningen avbröts av okänd anledning", true, new Exception("Payson betalning avbruten och ingen registrering i TempData hittades."));
            }

            var org = db.Organisationer.Include("Betalningsmetoder").Single(o => o.ID == registrering.Evenemang.OrganisationsId);

            var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);
#if DEBUG
            api = new PaysonApi("4", "2acab30d-fe50-426f-90d7-8c60a7eb31d4", ApplicationId, true);
#endif
            var response = api.MakePaymentDetailsRequest(new PaymentDetailsData(registrering.PaysonToken));

            db.Registreringar.Remove(registrering);
            log.Debug("Removed registrering with id: " + id);
            TempData["reg"] = registrering;

            return ShowPaymentError("Betalningen avbruten.", response.NvpContent, registrering);
        }

        public ActionResult IPN(int? id)
        {
            log.Debug("IPN regId: " + id);

            if (!id.HasValue)
            {
                return ShowError("Ett oväntat fel inträffade vid betalning. Kontrollera om ditt lag finns med i listan på anmälda lag.",
                    true, new Exception("Felaktigt id i IP. id: " + id));
            }

            var registration = db.Registreringar.FirstOrDefault(regg => regg.ID == id);

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

                if (status == PaymentStatus.Completed)
                {
                    log.Debug("IPN message, status: " + statusText + ". regId: " + id + " success: " + response.Success);

                    if (!registration.HarBetalt)
                    {
                        SetAsPaid(registration);
                    }
                }
                else
                {
                    log.Debug("IPN message for non complete transaction. regId: " + id + ". Status: " + statusText);
                }
            }
            else
            {
                log.Error("Got IPN with wrong regId as query parameter: " + id);
                SendMail.SendErrorMessage("Got IPN with wrong regId as query parameter: " + id);
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
            ViewBag.ev = registration.Evenemang.Namn;
            return RedirectToAction("Index");
        }
    }
}
