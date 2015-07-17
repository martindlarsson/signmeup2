using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    public class BetalningViewModel
    {
        public IList<TripletViewModel> Poster { get; set; }

        public BetalningViewModel(Banor bana, Kanoter kanot, RabattVM rabatt, ForseningsavgiftVM forseningsavgift)
        {
            Poster = new List<TripletViewModel>();
            Poster.Add(new TripletViewModel { TypNamn = "Bana", Namn = bana.Namn, Avgift = bana.Avgift });
            Poster.Add(new TripletViewModel { TypNamn = "Kanot", Namn = kanot.Namn, Avgift = kanot.Avgift });
            if (rabatt != null)
            {
                Poster.Add(new TripletViewModel { TypNamn = "Rabatt", Namn = rabatt.Kod, Avgift = -rabatt.Summa });
            }
            if (forseningsavgift != null)
            {
                string namn = "Från " + forseningsavgift.FranDatum.ToShortDateString() + " och till " + forseningsavgift.TillDatum.ToShortDateString();
                Poster.Add(new TripletViewModel
                {
                    TypNamn = forseningsavgift.Namn,
                    Namn = namn,
                    Avgift = forseningsavgift.PlusEllerMinus == TypAvgift.Avgift ? forseningsavgift.Summa : -forseningsavgift.Summa
                });
            }
        }

        public int SummaAttBetala
        {
            get
            {
                int summa = 0;
                if (Poster != null)
                {
                    foreach (var post in Poster)
                    {
                        summa += post.Avgift;
                    }
                }
                return summa < 0 ? 0 : summa;
            }
        }
    }

    public class TripletViewModel
    {
        public string TypNamn { get; set; }
        public string Namn { get; set; }
        public int Avgift { get; set; }
    }
}