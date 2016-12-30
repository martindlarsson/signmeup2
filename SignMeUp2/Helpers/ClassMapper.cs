using System;
using System.Collections.Generic;
using System.Linq;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;

namespace SignMeUp2.Helpers
{
    public class ClassMapper
    {
        public static Registrering MappaTillRegistrering(SignMeUpVM SUPVM)
        {
            // Mappa alla svar
            var faltsvar = new List<FaltSvar>();
            foreach(var steg in SUPVM.Steps)
            {
                foreach(var falt in steg.FaltLista)
                {
                    ValViewModel val = null;
                    if (falt.Avgiftsbelagd && falt.Typ == FaltTyp.val_falt)
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
                Invoice = SUPVM.Fakturaadress != null ? MappatillFakturaadress(SUPVM.Fakturaadress) : null,
                Registreringstid = DateTime.Now,
                PaysonToken = SUPVM.PaysonToken,
                AttBetala = SUPVM.AttBetala
            };
        }

        internal static Registrering MappaTillRegistrering(RegistreringVM registreringVM)
        {
            return new Registrering
            {
                AttBetala = registreringVM.AttBetala,
                FormularId = registreringVM.FormularId,
                Forseningsavgift = registreringVM.Forseningsavgift,
                HarBetalt = registreringVM.HarBetalt,
                Id = registreringVM.Id,
                Invoice = MappatillFakturaadress(registreringVM.Invoice), // TODO
                Kommentar = registreringVM.Kommentar,
                PaysonToken = registreringVM.PaysonToken,
                Rabatt = registreringVM.Rabatt,
                Rabattkod = registreringVM.Rabattkod,
                Registreringstid = registreringVM.Registreringstid,
                Svar = registreringVM.Svar.Select(svar => MappatillSvar(svar)).ToList()
            };
        }

        internal static RegistreringVM MappaTillRegistreringVM(Registrering registrering)
        {
            if (registrering == null) return null;

            return new RegistreringVM
            {
                AttBetala = registrering.AttBetala,
                FormularId = registrering.FormularId,
                Formular = registrering.Formular != null ? MappaTillFormularVM(registrering.Formular) : null,
                Forseningsavgift = registrering.Forseningsavgift,
                HarBetalt = registrering.HarBetalt,
                Id = registrering.Id,
                Invoice = registrering.Invoice != null ? MappatillFakturaadressVM(registrering.Invoice) : null,
                Kommentar = registrering.Kommentar,
                PaysonToken = registrering.PaysonToken,
                Rabatt = registrering.Rabatt,
                Rabattkod = registrering.Rabattkod,
                Registreringstid = registrering.Registreringstid,
                Svar = registrering.Svar != null ? registrering.Svar.Select(svar => MappatillSvarVM(svar)).ToList() : null
            };
        }

        private static FaltSvarVM MappatillSvarVM(FaltSvar svar)
        {
            return new FaltSvarVM
            {
                Avgift = svar.Avgift,
                Falt = svar.Falt != null ? MappaTillFaltVM(svar.Falt) : null,
                FaltId = svar.FaltId,
                Id = svar.Id,
                RegistreringsId = svar.RegistreringsId,
                Varde = svar.Varde
            };
        }

        private static FaltSvar MappatillSvar(FaltSvarVM svar)
        {
            return new FaltSvar
            {
                Avgift = svar.Avgift,
                Falt = svar.Falt != null ? MappaTillFalt(svar.Falt) : null,
                FaltId = svar.FaltId,
                Id = svar.Id,
                RegistreringsId = svar.RegistreringsId,
                Varde = svar.Varde
            };
        }

        private static FakturaadressVM MappatillFakturaadressVM(Fakturaadress invoice)
        {
            return new FakturaadressVM
            {
                Att = invoice.Att,
                Box = invoice.Box,
                Id = invoice.Id,
                Namn = invoice.Namn,
                Organisationsnummer = invoice.Organisationsnummer,
                Postadress = invoice.Postadress,
                Postnummer = invoice.Postnummer,
                Postort = invoice.Postort
            };
        }

        private static Fakturaadress MappatillFakturaadress(FakturaadressVM invoice)
        {
            return new Fakturaadress
            {
                Att = invoice.Att,
                Box = invoice.Box,
                Id = invoice.Id,
                Namn = invoice.Namn,
                Organisationsnummer = invoice.Organisationsnummer,
                Postadress = invoice.Postadress,
                Postnummer = invoice.Postnummer,
                Postort = invoice.Postort
            };
        }

        internal static FormularViewModel MappaTillFormularVM(Formular formular)
        {
            return new FormularViewModel
            {
                Aktivitet = formular.Aktivitet != null ? MappaTillAktivitetVM(formular.Aktivitet) : null,
                AktivitetsId = formular.AktivitetsId,
                Avgift = formular.Avgift,
                Evenemang = formular.Evenemang != null ? MappaTillEvenemangVM(formular.Evenemang) : null,
                EvenemangsId = formular.EvenemangsId,
                Listor = formular.Listor,
                Id = formular.Id,
                MaxRegistreringar = formular.MaxRegistreringar,
                Namn = formular.Namn,
                Publikt = formular.Publikt,
                //Registreringar = formular.Registreringar.Select(reg => MappaTillRegistreringVM(reg)).ToList(),
                Steg = formular.Steg.Select(steg => MappaTillStegVM(steg, formular.Steg.Count)).ToList()
            };
        }

        internal static Formular MappaTillFormular(FormularViewModel formularVM)
        {
            return new Formular
            {
                AktivitetsId = formularVM.AktivitetsId,
                Avgift = formularVM.Avgift,
                EvenemangsId = formularVM.EvenemangsId,
                Listor = formularVM.Listor,
                Id = formularVM.Id,
                MaxRegistreringar = formularVM.MaxRegistreringar,
                Namn = formularVM.Namn,
                Publikt = formularVM.Publikt,
                //Registreringar = formularVM.Registreringar.Select(reg => MappaTillRegistrering(reg)).ToList(),
                Steg = formularVM.Steg.Select(steg => MappaTillSteg(steg)).ToList()
            };
        }

        private static FormularSteg MappaTillSteg(FormularStegVM steg)
        {
            return new FormularSteg
            {
                Falt = steg.FaltLista.Select(falt => MappaTillFalt(falt)).ToList(),
                //FormularId = steg.FormularId,
                Id = steg.Id,
                Index = steg.StepIndex,
                Namn = steg.Namn
            };
        }

        private static FormularStegVM MappaTillStegVM(FormularSteg steg, int count)
        {
            return new FormularStegVM
            {
                Id = steg.Id,
                Namn = steg.Namn,
                StepIndex = steg.Index,
                StepCount = count,
                FaltLista = steg.Falt.Select(falt => MappaTillFaltVM(falt)).ToList()
            };
        }

        internal static Forseningsavgift MappaTillForseningsavgift(ForseningsavgiftVM forseningsavgift)
        {
            return new Forseningsavgift
            {
                EvenemangsId = forseningsavgift.EvenemangsId,
                FranDatum = forseningsavgift.FranDatum,
                Id = forseningsavgift.Id,
                Namn = forseningsavgift.Namn,
                PlusEllerMinus = forseningsavgift.PlusEllerMinus,
                Summa = forseningsavgift.Summa,
                TillDatum = forseningsavgift.TillDatum
            };
        }

        internal static ForseningsavgiftVM MappaTillForseningsavgiftVM(Forseningsavgift forseningsavgift)
        {
            return new ForseningsavgiftVM
            {
                EvenemangsId = forseningsavgift.EvenemangsId.Value,
                FranDatum = forseningsavgift.FranDatum,
                Id = forseningsavgift.Id,
                Namn = forseningsavgift.Namn,
                PlusEllerMinus = forseningsavgift.PlusEllerMinus,
                Summa = forseningsavgift.Summa,
                TillDatum = forseningsavgift.TillDatum
            };
        }

        private static AktivitetViewModel MappaTillAktivitetVM(Aktivitet aktivitet)
        {
            return new AktivitetViewModel
            {
                Id = aktivitet.Id,
                Namn = aktivitet.Namn
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

        internal static RabattVM MappaTillRabattVM(Rabatter rabatter)
        {
            return new RabattVM
            {
                Beskrivning = rabatter.Beskrivning,
                Id = rabatter.Id,
                Kod = rabatter.Kod,
                Summa = rabatter.Summa,
                EvenemangsId = rabatter.EvenemangsId
            };
        }

        internal static Rabatter MappaTillRabatt(RabattVM rabatt)
        {
            return new Rabatter
            {
                Beskrivning = rabatt.Beskrivning,
                EvenemangsId = rabatt.EvenemangsId,
                Id = rabatt.Id,
                Kod = rabatt.Kod,
                Summa = rabatt.Summa
            };
        }

        internal static Evenemang MappaTillEvenemang(EvenemangVM evenemang)
        {
            return new Evenemang
            {
                Id = evenemang.Id,
                FakturaBetaldSenast = evenemang.FakturaBetaldSenast,
                Fakturabetalning = evenemang.Fakturabetalning,
                Namn = evenemang.Namn,
                OrganisationsId = evenemang.OrganisationsId,
                RegStart = evenemang.RegStart,
                RegStop = evenemang.RegStop,
                Språk = evenemang.Språk == ViewModels.Språk.Svenska ? Data.Språk.Svenska : Data.Språk.Engelska
            };
        }

        private static EvenemangVM MappaTillEvenemangVM(Evenemang evenemang)
        {
            return new EvenemangVM
            {
                Id = evenemang.Id,
                FakturaBetaldSenast = evenemang.FakturaBetaldSenast,
                Fakturabetalning = evenemang.Fakturabetalning,
                Namn = evenemang.Namn,
                OrganisationsId = evenemang.OrganisationsId,
                RegStart = evenemang.RegStart,
                RegStop = evenemang.RegStop,
                Språk = evenemang.Språk == Data.Språk.Svenska ? ViewModels.Språk.Svenska : ViewModels.Språk.Engelska
            };
        }

        private static FaltViewModel MappaTillFaltVM(Falt falt)
        {
            return new FaltViewModel
            {
                Avgiftsbelagd = falt.Avgiftsbelagd,
                Kravs = falt.Kravs,
                Namn = falt.Namn,
                Typ = falt.Typ,
                Val = falt.Val.Select(val => MappaTillValVM(val, falt.Namn)).ToList(),
                FaltId = falt.Id
            };
        }

        private static Falt MappaTillFalt(FaltViewModel faltVM)
        {
            return new Falt
            {
                Avgiftsbelagd = faltVM.Avgiftsbelagd,
                Kravs = faltVM.Kravs,
                Namn = faltVM.Namn,
                Typ = faltVM.Typ,
                Val = faltVM.Val.Select(val => MappaTillVal(val)).ToList(),
                Id = faltVM.FaltId
            };
        }

        private static ValViewModel MappaTillValVM(Val val, string typnamn)
        {
            return new ValViewModel
            {
                Avgift = val.Avgift,
                Id = val.Id,
                Namn = val.Namn,
                TypNamn = typnamn
            };
        }
        
        private static Val MappaTillVal(ValViewModel val)
        {
            return new Val
            {
                Avgift = val.Avgift,
                Id = val.Id,
                Namn = val.Namn
                //TypNamn = val.TypNamn
            };
        }

        public static FakturaadressVM MappTillFakturaadressVM(Fakturaadress invoice)
        {
            return new FakturaadressVM
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