//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace SignMeUp2.Models
//{
//    public class BekraftaRegistreringViewModel
//    {
//        public BekraftaRegistreringViewModel()
//        {
//            Betalnignsposter = new List<TripletViewModel>();
//        }

//        public IList<TripletViewModel> Betalnignsposter { get; set; }

//        public WizardViewModel Wizard { get; set; }

//        public string Rabattkod { get; set; }

//        public int SummaAttBetala
//        {
//            get
//            {
//                int summa = 0;
//                foreach (var post in Betalnignsposter)
//                {
//                    summa += post.Avgift;
//                }
//                return summa < 0 ? 0 : summa;
//            }
//        }
//    }
//}

//public class TripletViewModel {
//    public string Typ { get; set; }
//    public string Namn { get; set; }
//    public int Avgift { get; set; }
//}