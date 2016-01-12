using System;
using SignMeUp2.Data;
using SignMeUp2.Models;

namespace SignMeUp2.Helpers
{
    public class Avgift
    {
        public static int Kalk(Registrering registrering)
        {
            int tot = 0;

            foreach (var svar in registrering.Svar)
            {
                tot += svar.Avgift;
            }

            return tot
                + registrering.Forseningsavgift
                - registrering.Rabatt;
        }
    }
}