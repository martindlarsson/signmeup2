using SignMeUp2.Data;
using System;
using System.Collections.Generic;

namespace SignMeUp2.ViewModels
{
    public class FakturaVM
    {
        public string Evenemangsnamn { get; set; }

        public Registreringar Registrering { get; set; }

        public InvoiceViewModel Fakturaadress { get; set; }

        public DateTime? BetalaSenast { get; set; }

        public Organisation Arrangor { get; set; }

        public BetalningsmetoderVM Betalningsmetoder { get; set; }

        public IList<ValViewModel> Betalnignsposter
        {
            get
            {
                var list = new List<ValViewModel>();
                
                // Bana
                
                list.Add(new ValViewModel { TypNamn = "Bana", Namn = Registrering.Bana.Namn, Avgift = Registrering.Bana.Avgift });

                // Kanot
                list.Add(new ValViewModel { TypNamn = "Kanot", Namn = Registrering.Kanot.Namn, Avgift = Registrering.Kanot.Avgift });

                // Rabatt
                if (Registrering.Rabatt != null)
                {
                    list.Add(new ValViewModel { TypNamn = "Rabatt", Namn = Registrering.Rabatt.Kod, Avgift = -Registrering.Rabatt.Summa });
                }

                // Förseningsavgift
                if (Registrering.Forseningsavgift != null)
                {
                    var summa = Registrering.Forseningsavgift.PlusEllerMinus == TypAvgift.Avgift ? Registrering.Forseningsavgift.Summa : -Registrering.Forseningsavgift.Summa;
                    list.Add(new ValViewModel { TypNamn = Registrering.Forseningsavgift.PlusEllerMinus.ToString(), Namn = Registrering.Forseningsavgift.Namn, Avgift = summa });
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