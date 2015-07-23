//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using SignMeUp2.Data;

//namespace SignMeUp2.ViewModels
//{
//    public class BetalningViewModel
//    {
//        public IList<ValViewModel> Poster { get; set; }

//        public BetalningViewModel(Banor bana, Kanoter kanot, RabattVM rabatt, ForseningsavgiftVM forseningsavgift)
//        {
//            Poster = new List<ValViewModel>();
//            Poster.Add(new ValViewModel { TypNamn = "Bana", Namn = bana.Namn, Avgift = bana.Avgift });
//            Poster.Add(new ValViewModel { TypNamn = "Kanot", Namn = kanot.Namn, Avgift = kanot.Avgift });
//            if (rabatt != null)
//            {
//                Poster.Add(new ValViewModel { TypNamn = "Rabatt", Namn = rabatt.Kod, Avgift = -rabatt.Summa });
//            }
//            if (forseningsavgift != null)
//            {
//                string namn = "Från " + forseningsavgift.FranDatum.ToShortDateString() + " och till " + forseningsavgift.TillDatum.ToShortDateString();
//                Poster.Add(new ValViewModel
//                {
//                    TypNamn = forseningsavgift.Namn,
//                    Namn = namn,
//                    Avgift = forseningsavgift.PlusEllerMinus == TypAvgift.Avgift ? forseningsavgift.Summa : -forseningsavgift.Summa
//                });
//            }
//        }

//        public int SummaAttBetala
//        {
//            get
//            {
//                int summa = 0;
//                if (Poster != null)
//                {
//                    foreach (var post in Poster)
//                    {
//                        summa += post.Avgift;
//                    }
//                }
//                return summa < 0 ? 0 : summa;
//            }
//        }
//    }
//}