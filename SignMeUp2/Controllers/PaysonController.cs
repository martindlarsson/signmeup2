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
using SignMeUp2.Data;
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
            // Om PaysonViewModel finns i TempData kommmer vi från Cancelled och ska visa
            // felmeddelande
            if (TempData[PaysonViewModel.PAYSON_VM] != null)
            {
                var paysonVM = TempData[PaysonViewModel.PAYSON_VM] as PaysonViewModel;
                TempData[PaysonViewModel.PAYSON_VM] = paysonVM;

                ViewBag.ev = paysonVM.Registrering.Evenemang.Namn;
                return View(paysonVM.Kontaktinformation);
            }

            var reg = (Registreringar)TempData["reg"];
            smuService.FillRegistrering(reg);

            log.Debug("Paysonbetalning påbörjad för lag " + reg.Lagnamn);

            if (reg == null)
            {   
                return RedirectToAction("Index", "SignMeUp");
            }

            var paysonViewModel = new PaysonViewModel { Registrering = reg };

            var paysonKontaktinfo = new PaysonKontaktViewModel
            {
                SenderEmail = reg.Epost,
                SenderFirstName = reg.Deltagare.FirstOrDefault().Förnamn,
                SenderLastName = reg.Deltagare.FirstOrDefault().Efternamn
            };
            paysonViewModel.Kontaktinformation = paysonKontaktinfo;

            TempData[PaysonViewModel.PAYSON_VM] = paysonViewModel;

            ViewBag.ev = reg.Evenemang.Namn;
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
                    var reg = paysonViewModel.Registrering;

                    if (paysonViewModel == null || reg == null)
                    {
                        return ShowError("Oväntat fel. Var god försök senare", true, new Exception("Ingen paysonViewModel i TempData."));
                    }

                    paysonViewModel.Kontaktinformation = kontaktInfo;

                    log.Debug("Payment: Lagnamn: " + reg.Lagnamn);

                    // Spara temporärt i databasen
                    smuService.SparaNyRegistrering(reg);
                    smuService.FillRegistrering(reg);

                    paysonViewModel.RegId = reg.Id;

                    var orgId = paysonViewModel.Registrering.Evenemang.OrganisationsId;
                    var org = smuService.Db.Organisationer.Include("Betalningsmetoder").Single(o => o.Id == orgId);

                    PayData payData = SkapaPaysonPayData(paysonViewModel, org);

                    var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);
#if DEBUG
                    api = new PaysonApi("4", "2acab30d-fe50-426f-90d7-8c60a7eb31d4", ApplicationId, true);
#endif                    
                    var response = api.MakePayRequest(payData);

                    if (response.Success)
                    {
                        paysonViewModel.Token = response.Token;
                        paysonViewModel.Registrering.PaysonToken = response.Token;
                        smuService.UpdateraRegistrering(paysonViewModel.Registrering);

                        var forwardUrl = api.GetForwardPayUrl(response.Token);

                        TempData[PaysonViewModel.PAYSON_VM] = paysonViewModel;
                        TempData["reg"] = paysonViewModel.Registrering;

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

            TempData["reg"] = paysonViewModel.Registrering;
            ViewBag.ev = paysonViewModel.Registrering.Evenemang.Namn;
            return View(kontaktInfo);
        }

        private PayData SkapaPaysonPayData(PaysonViewModel paysonViewModel, Organisation org)
        {
            // We remove port info to help when the site is behind a load balancer/firewall that does port rewrites.
            var scheme = Request.Url.Scheme;
            var host = Request.Url.Host;

            var returnUrl = Url.Action("Returned", "Payson", new RouteValueDictionary(), scheme, host) + "/" + paysonViewModel.RegId;

            var cancelUrl = Url.Action("Cancelled", "Payson", new RouteValueDictionary(), scheme, host) + "/" + paysonViewModel.RegId;

            var sender = new Sender(paysonViewModel.Kontaktinformation.SenderEmail);
            sender.FirstName = paysonViewModel.Kontaktinformation.SenderFirstName;
            sender.LastName = paysonViewModel.Kontaktinformation.SenderLastName;

            var reg = paysonViewModel.Registrering;
            smuService.FillRegistrering(reg);
            var betalningVM = new BetalningViewModel(reg.Bana, reg.Kanot, reg.Rabatt, reg.Forseningsavgift);

            var receiver = new Receiver(org.Epost, betalningVM.SummaAttBetala);
            receiver.FirstName = org.Namn;
            //receiver.LastName = PaysonViewModel.PaysonRecieverLastName;
            receiver.SetPrimaryReceiver(true);

            var payData = new PayData(returnUrl,
                cancelUrl,
                smuService.HamtaEvenemang(paysonViewModel.Registrering.EvenemangsId.Value).Namn + " - " + paysonViewModel.Registrering.Lagnamn,
                sender,
                new List<Receiver> { receiver });

            // Set IPN callback URL
            // When the shop is hosted by Payson the IPN scheme must be http and not https
            var ipnNotificationUrl = Url.Action("IPN", "Payson", new RouteValueDictionary(), scheme, host) + "/" + paysonViewModel.RegId;
            payData.SetIpnNotificationUrl(ipnNotificationUrl);

            payData.SetFundingConstraints(new List<FundingConstraint> { FundingConstraint.Bank, FundingConstraint.CreditCard });
            payData.SetTrackingId(paysonViewModel.Registrering.Id.ToString());

            // Skapa poster för betalning
            var orderItems = new List<PaysonIntegration.Utils.OrderItem>();
            foreach (TripletViewModel post in betalningVM.Poster)
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
            log.Debug("Returned");

            if (!id.HasValue)
            {
                ShowError("Ett fel inträffade när betalningen slutfördes. Kontrollera i startlistan om er registrering genomförts", true, new Exception("Felaktigt angivet regId: " + id));
            }

            var registration = smuService.Db.Registreringar.Find(id);

            if (registration == null)
            {
                ShowError("Ett fel inträffade när betalningen slutfördes. Kontrollera i startlistan om er registrering genomförts", true, new Exception("Ingen registration hittad med  id: " + id + " i Returned."));
            }

            log.Debug("Returned. Lagnamn: " + registration.Lagnamn);

            // If no payment message has been sent (IPN)
            if (!registration.HarBetalt)
            {
                var org = smuService.Db.Organisationer.Include("Betalningsmetoder").Single(o => o.Id == registration.Evenemang.OrganisationsId);

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
                        smuService.HarBetalt(registration);
                        SkickaRegMail(registration);
                        TempData[PaysonViewModel.PAYSON_VM] = null;
                    }
                }
                else
                {
                    log.Warn("Deleting temp-registration with id: " + id);

                    // Remove the temporary registration
                    smuService.TabortRegistrering(registration);

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

            var registrering = smuService.Db.Registreringar.Find(id);

            if (registrering == null)
            {
                return ShowError("Betalningen avbröts av okänd anledning", true, new Exception("Payson betalning avbruten och ingen registrering i TempData hittades."));
            }

            var org = smuService.Db.Organisationer.Include("Betalningsmetoder").Single(o => o.Id == registrering.Evenemang.OrganisationsId);

            var api = new PaysonApi(org.Betalningsmetoder.PaysonUserId, org.Betalningsmetoder.PaysonUserKey, ApplicationId, false);
#if DEBUG
            api = new PaysonApi("4", "2acab30d-fe50-426f-90d7-8c60a7eb31d4", ApplicationId, true);
#endif
            var response = api.MakePaymentDetailsRequest(new PaymentDetailsData(registrering.PaysonToken));

            // Ta bort temporär registrering
            smuService.TabortRegistrering(registrering);
            registrering.PaysonToken = null;

            var paysonVM = TempData[PaysonViewModel.PAYSON_VM] as PaysonViewModel;
            if (paysonVM != null)
            {
                paysonVM.Registrering.PaysonToken = null;
                TempData[PaysonViewModel.PAYSON_VM] = paysonVM;
            }

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

            var registration = smuService.Db.Registreringar.Find(id);

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
                        smuService.HarBetalt(registration);
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

            //if (registration != null)
            //{
            //    TempData["reg"] = registration;
            //    smuService.FillRegistrering(registration);
            //    ViewBag.ev = registration.Evenemang.Namn;
            //}

            return RedirectToAction("Index");
        }
    }
}
