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
                PaysonToken = SUPVM.PaysonToken,
                HarBetalt = SUPVM.AttBetala == 0 ? true : false
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

        public static InvoiceViewModel MappTillInvoiceVM(Invoice invoice)
        {
            return new InvoiceViewModel
            {
                Organisationsnummer = invoice.Organisationsnummer,
                Namn = invoice.Namn,
                Att = invoice.Att,
                Box = invoice.Box,
                Id = invoice.Id,
                Postadress = invoice.Postadress,
                Postnummer = invoice.Postnummer,
                Postort = invoice.Postort
            };
        }

        internal static BetalningsmetoderVM MappaTillBetalningsmetoderVM(Betalningsmetoder bm)
        {
            return new BetalningsmetoderVM
            {
                BIC = bm.BIC,
                Gironummer = bm.Gironummer,
                GiroTyp = bm.GiroTyp,
                HarPayson = bm.HarPayson,
                IBAN = bm.IBAN,
                Id = bm.Id,
                KanTaEmotIntBetalningar = bm.KanTaEmotIntBetalningar,
                OrganisationsId = bm.Organisation.Id,
                PaysonUserId = bm.PaysonUserId,
                PaysonUserKey = bm.PaysonUserKey
            };
        }
    }
}