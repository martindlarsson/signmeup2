using System;
using System.Collections.Generic;
using System.Linq;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using SignMeUp2.Services;

namespace SignMeUp2.Helpers
{
    public class ClassMapper
    {
        public static Registrering MappaTillRegistrering(SignMeUpVM SUPVM, SignMeUpService SMU)
        {
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

            var forseningsavgift = 0;
            if (SUPVM.FAVM != null)
            {
                forseningsavgift = SUPVM.FAVM.PlusEllerMinus == TypAvgift.Rabatt ? -SUPVM.FAVM.Summa : SUPVM.FAVM.Summa;
            }

            return new Registrering
            {
                FormularId = SUPVM.FormularsId,
                Svar = faltsvar,
                Rabatt = SUPVM.Rabatt != null ? SUPVM.Rabatt.Summa : 0,
                Rabattkod = SUPVM.Rabatt != null ? SUPVM.Rabatt.Kod : string.Empty,
                Forseningsavgift = forseningsavgift,
                Invoice = SUPVM.Fakturaadress != null ? MappTillInvoice(SUPVM.Fakturaadress) : null,
                Registreringstid = DateTime.Now,
                PaysonToken = SUPVM.PaysonToken,
                AttBetala = SUPVM.AttBetala
            };
        }

        internal static TabellViewModel MappaTillTabell(Lista listan)
        {
            var tabell = new TabellViewModel { Namn = listan.Namn };
            var kolumner = new List<Kolumn>();
            
            // Skapa kolumnerna
            foreach(var falt in listan.Falt)
            {
                kolumner.Add(new Kolumn { Rubrik = falt.Falt.Namn, FaltId = falt.FaltId.Value, Index = falt.Index });
            }
            
            tabell.Kolumner = kolumner.OrderBy(k => k.Index).ToList();
            kolumner = null;
            
            // Hämta svaren
            foreach (var registrering in listan.Formular.Registreringar)
            {
                var rad = new Rad();
                foreach (var kolumn in tabell.Kolumner)
                {
                    var svar = registrering.Svar.Single(s => s.FaltId == kolumn.FaltId);
                    var textSvar = svar.Varde;
                    // Om det är val, slå upp namnet på valet
                    if (svar.Falt.Typ == FaltTyp.val_falt)
                    {
                        textSvar = svar.Falt.Val.Single(v => v.Id.ToString() == svar.Varde).Namn;
                    }
                    rad.Varden.Add(textSvar);
                }
                tabell.Rader.Add(rad);
            }

            return tabell;
        }

        public static FormularViewModel MappaTillFormular(Formular formular)
        {
            return new FormularViewModel
            {
                Evenemang = formular.Evenemang,
                EvenemangsId = formular.EvenemangsId,
                Id = formular.Id,
                Namn = formular.Namn,
                Avgift = formular.Avgift,
                Steg = MappaTillSteg(formular.Steg)
            };
        }

        private static List<ViewModels.FormularSteg> MappaTillSteg(ICollection<Data.FormularSteg> stegs)
{
            var list = new List<ViewModels.FormularSteg>();

            foreach (var steg in stegs)
            {
                list.Add(new ViewModels.FormularSteg
                {
                    Id = steg.Id,
                    Namn = steg.Namn,
                    StepIndex = steg.Index,
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

        public static Fakturaadress MappTillInvoice(InvoiceViewModel invoiceVM)
        {
            return new Fakturaadress
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

        public static InvoiceViewModel MappTillInvoiceVM(Fakturaadress invoice)
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