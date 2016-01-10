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
            //var banId = int.Parse(SUPVM.GetFaltvarde("Bana"));
            //var bana = SMU.Db.Banor.Find(banId);

            //var klassId = int.Parse(SUPVM.GetFaltvarde("Klass"));
            //var klass = SMU.Db.Klasser.Find(klassId);

            //var kanotId = int.Parse(SUPVM.GetFaltvarde("Kanot"));
            //var kanot = SMU.Db.Kanoter.Find(kanotId);

            //var deltagareSteg = SUPVM.GetStep("Deltagare");
            //int antalDeltagare = deltagareSteg.FaltLista.Count / 2;
            //var deltagare = new List<Deltagare>();
            //for (int i = 1; i <= antalDeltagare; i++)
            //{
            //    deltagare.Add(new Deltagare
            //    {
            //        Förnamn = SUPVM.GetFaltvarde("Förnamn " + i),
            //        Efternamn = SUPVM.GetFaltvarde("Efternamn " + i)
            //    });
            //} 

            int? rabattId = null;
            if (SUPVM.Rabatt != null)
                rabattId = SUPVM.Rabatt.Id;

            int? forseningsavgId = null;
            if (SUPVM.FAVM != null)
                forseningsavgId = SUPVM.FAVM.Id;

            // Mappa alla svar
            var faltsvar = new List<FaltSvar>();
            foreach(var steg in SUPVM.Steps)
            {
                foreach(var falt in steg.FaltLista)
                {
                    ValViewModel val = null;
                    if (falt.Avgiftsbelagd && falt.Typ == Data.FaltTyp.val_falt)
                    {
                        val = falt.Val.FirstOrDefault(v => v.Id == int.Parse(falt.Varde));
                    }

                    faltsvar.Add(new FaltSvar
                    {
                        Avgift = val != null ? val.Avgift : 0,
                        FaltId = falt.FaltId,
                        Varde = falt.Varde
                    });
                }
            }

            return new Registreringar
            {
                FormularsId = SUPVM.FormularsId,
                //Lagnamn = SUPVM.GetFaltvarde("Lagnamn"),
                //Bana = bana,
                //Klass = klass,
                //Kanot = kanot,
                //Klubb = SUPVM.GetFaltvarde("Klubb"),
                //Deltagare = deltagare.ToList(),
                //Adress = SUPVM.GetFaltvarde("Adress"),
                //Telefon = SUPVM.GetFaltvarde("Telefon"),
                // TODO sätt rabatt och förseningsavgift som ints istället för FK
                Svar = faltsvar,
                RabattId = rabattId,
                ForseningsavgiftId = forseningsavgId,
                Invoice = SUPVM.Fakturaadress != null ? MappTillInvoice(SUPVM.Fakturaadress) : null,
                Registreringstid = DateTime.Now,
                PaysonToken = SUPVM.PaysonToken
            };
        }

        public static FormularViewModel MappaTillFormular(Formular formular)
        {
            return new FormularViewModel
            {
                Evenemang = formular.Evenemang,
                EvenemangsId = formular.EvenemangsId,
                Gratis = formular.Gratis,
                Id = formular.Id,
                Namn = formular.Namn,
                Startavgift = formular.Startavgift,
                Steg = MappaTillSteg(formular.Steg)
            };
        }

        private static List<ViewModels.WizardStep> MappaTillSteg(ICollection<Data.WizardStep> stegs)
        {
            var list = new List<ViewModels.WizardStep>();

            foreach (var steg in stegs)
            {
                list.Add(new ViewModels.WizardStep
                {
                    Namn = steg.Namn,
                    StepIndex = steg.StepIndex,
                    StepCount = stegs.Count(),
                    FaltLista = MappaTillFalt(steg.Falt)
                });
            }

            return list;
        }

        private static ICollection<FaltViewModel> MappaTillFalt(ICollection<Falt> falts)
        {
            var list = new List<FaltViewModel>();

            foreach (var falt in falts)
            {
                list.Add(new FaltViewModel
                {
                    Avgiftsbelagd = falt.Avgiftsbelagd,
                    Kravs = falt.Kravs,
                    Namn = falt.Namn,
                    Typ = falt.Typ,
                    Val = MappTillVal(falt.Val),
                    FaltId = falt.Id
                });
            }

            return list;
        }

        private static IList<ValViewModel> MappTillVal(ICollection<Val> vals)
        {
            var list = new List<ValViewModel>();

            foreach(var val in vals)
            {
                list.Add(new ValViewModel
                {
                    Avgift = val.Avgift,
                    Id = val.Id,
                    Namn = val.Namn,
                    TypNamn = val.TypNamn
                });
            }

            return list;
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