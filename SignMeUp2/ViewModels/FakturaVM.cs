using SignMeUp2.Data;
using System;
using System.Collections.Generic;

namespace SignMeUp2.ViewModels
{
    public class FakturaVM
    {
        public string Evenemangsnamn { get; set; }

        public Registrering Registrering { get; set; }

        public InvoiceViewModel Fakturaadress { get; set; }

        public DateTime? BetalaSenast { get; set; }

        public Organisation Arrangor { get; set; }

        public BetalningsmetoderVM Betalningsmetoder { get; set; }

        public IList<ValViewModel> Betalnignsposter
        {
            get
            {
                var list = new List<ValViewModel>();

                if (Registrering.Formular.Avgift > 0)
                {
                    list.Add(new ValViewModel { TypNamn = "Grundavgift", Namn = "Anmälan", Avgift = Registrering.Formular.Avgift });
                }
                
                foreach (var svar in Registrering.Svar)
                {
                    if (svar.Falt.Avgiftsbelagd)
                        list.Add(new ValViewModel { TypNamn = svar.Falt.Typ.ToString(), Namn = svar.Falt.Namn, Avgift = svar.Avgift });
                }

                // Rabatt
                if (Registrering.Rabatt != 0)
                {
                    list.Add(new ValViewModel { TypNamn = "Rabatt", Namn = Registrering.Rabattkod, Avgift = -Registrering.Rabatt });
                }

                // Förseningsavgift
                if (Registrering.Forseningsavgift != 0)
                {
                    //var summa = Registrering.Forseningsavgift.PlusEllerMinus == TypAvgift.Avgift ? Registrering.Forseningsavgift.Summa : -Registrering.Forseningsavgift.Summa;
                    list.Add(new ValViewModel { TypNamn = "Avgift" /*Registrering.Forseningsavgift.PlusEllerMinus.ToString()*/, Namn = "Sen anmälan" /*Registrering.Forseningsavgift.Namn*/, Avgift = Registrering.Forseningsavgift });
                }

                return list;
            }
        }

        public int AttBetala
        {
            get
            {
                int summa = 0;
                foreach (var val in Betalnignsposter)
                {
                    summa += val.Avgift;
                }

                return summa < 0 ? 0 : summa;
            }
        }

    }
}