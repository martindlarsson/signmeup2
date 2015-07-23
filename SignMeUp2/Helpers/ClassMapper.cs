using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using SignMeUp2.Services;

namespace SignMeUp2.Helpers
{
    public class ClassMapper
    {
        public static Registreringar MappaTillRegistrering(SignMeUpVM SUPVM, SignMeUpService SMU)
        {
            var banId = int.Parse(SUPVM.GetFaltvarde("Bana"));
            var bana = SMU.Db.Banor.Find(banId);

            var klassId = int.Parse(SUPVM.GetFaltvarde("Klass"));
            var klass = SMU.Db.Klasser.Find(klassId);

            var kanotId = int.Parse(SUPVM.GetFaltvarde("Kanot"));
            var kanot = SMU.Db.Kanoter.Find(kanotId);

            var deltagareSteg = SUPVM.GetStep("Deltagare");
            int antalDeltagare = deltagareSteg.FaltLista.Count / 2;
            var deltagare = new List<Deltagare>();
            for (int i = 1; i <= antalDeltagare; i++)
            {
                deltagare.Add(new Deltagare
                {
                    Förnamn = SUPVM.GetFaltvarde("Förnamn " + i),
                    Efternamn = SUPVM.GetFaltvarde("Efternamn " + i)
                });
            } 

            int? rabattId = null;
            if (SUPVM.Rabatt != null)
                rabattId = SUPVM.Rabatt.Id;

            int? forseningsavgId = null;
            if (SUPVM.FAVM != null)
                forseningsavgId = SUPVM.FAVM.Id;

            return new Registreringar
            {
                EvenemangsId = SUPVM.EvenemangsId,
                Lagnamn = SUPVM.GetFaltvarde("Lagnamn"),
                Bana = bana,
                Klass = klass,
                Kanot = kanot,
                Klubb = SUPVM.GetFaltvarde("Klubb"),
                Deltagare = deltagare.ToList(),
                Adress = SUPVM.GetFaltvarde("Adress"),
                Telefon = SUPVM.GetFaltvarde("Telefon"),
                Epost = SUPVM.GetFaltvarde("Epost"),
                RabattId = rabattId,
                ForseningsavgiftId = forseningsavgId,
                Invoice = SUPVM.Fakturaadress != null ? MappTillInvoice(SUPVM.Fakturaadress) : null,
                Registreringstid = DateTime.Now,
                PaysonToken = SUPVM.PaysonToken
            };
        }

        public static Invoice MappTillInvoice(InvoiceViewModel invoiceVM)
        {
            return new Invoice
            {
                Att = invoiceVM.Att,
                Box = invoiceVM.Box,
                Id = invoiceVM.Id,
                Namn = invoiceVM.Namn,
                Organisationsnummer = invoiceVM.Organisationsnummer,
                Postadress = invoiceVM.Postadress,
                Postnummer = invoiceVM.Postnummer,
                Postort = invoiceVM.Postort
            };
        }

        //public static Registreringar MapToRegistreringar(WizardViewModel wizard)
        //{
        //    var reg = new Registreringar();

        //    reg.EvenemangsId = wizard.Evenemang_Id;

        //    reg.Invoice = wizard.Fakturaadress;

        //    reg.Rabatt = wizard.Rabatt;

        //    reg.Forseningsavgift = wizard.Forseningsavgift;

        //    foreach (IWizardStep step in wizard.Steps)
        //    {
        //        if (step is RegistrationViewModel)
        //        {
        //            var regStep = (RegistrationViewModel)step;
        //            reg.Lagnamn = regStep.Lagnamn;
        //            reg.Bana = regStep.Banor;
        //            reg.Klass = regStep.Klasser;
        //            reg.Kanot = regStep.Kanoter;
        //            reg.Ranking = regStep.Ranking;
        //        }
        //        else if (step is ContactViewModel)
        //        {
        //            var contStep = (ContactViewModel)step;
        //            reg.Adress = contStep.Adress;
        //            reg.Epost = contStep.Epost;
        //            reg.Telefon = contStep.Telefon;
        //            reg.Klubb = contStep.Klubb;
        //        }
        //        else if (step is DeltagareListViewModel)
        //        {
        //            reg.Deltagare = new List<Deltagare>();
        //            var deltagarlistaStep = (DeltagareListViewModel)step;
        //            foreach (var deltagareNew in deltagarlistaStep.DeltagareLista)
        //            {
        //                var deltagare = new Deltagare();
        //                deltagare.Förnamn = deltagareNew.Förnamn;
        //                deltagare.Efternamn = deltagareNew.Efternamn;
        //                reg.Deltagare.Add(deltagare);
        //            }
        //        }
        //    }

        //    return reg;
        //}
    }
}