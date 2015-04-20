using System;
using SignMeUp2.DataModel;

namespace SignMeUp2.Helpers
{
    public class Avgift
    {
        public static int Kalk(Registreringar registrering)
        {
            var avgift = registrering.Banor.Avgift
                + (registrering.Kanoter != null ? registrering.Kanoter.Avgift : 0)
                + registrering.Forseningsavgift
                - registrering.Rabatter;
            return avgift != null ? avgift.Value : 0;
        }

        public static int Forseningsavgift(SignMeUpDataModel db)
        {
            var avgift = 0;

            foreach (Forseningsavgift forseningsavgift in db.Forseningsavgift)
            {
                if (DateTime.Now > forseningsavgift.FranDatum)
                {
                    avgift = forseningsavgift.Summa;   
                }
            }
            return avgift;
        }
    }
}