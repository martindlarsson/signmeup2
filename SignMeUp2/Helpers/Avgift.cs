using System;
using SignMeUp2.DataModel;
using SignMeUp2.Models;

namespace SignMeUp2.Helpers
{
    public class Avgift
    {
        public static int Kalk(Registreringar registrering)
        {
            return registrering.Bana.Avgift
                + (registrering.Kanot != null ? registrering.Kanot.Avgift : 0)
                + registrering.Forseningsavgift.Summa
                - registrering.Rabatt.Summa;
        }



        //public static int Forseningsavgift(SignMeUpDataModel db)
        //{
        //    var avgift = 0;

        //    foreach (Forseningsavgift forseningsavgift in db.Forseningsavgift)
        //    {
        //        if (DateTime.Now > forseningsavgift.FranDatum)
        //        {
        //            avgift = forseningsavgift.Summa;   
        //        }
        //    }
        //    return avgift;
        //}
    }
}