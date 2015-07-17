using System.Collections.Generic;
using System.Web.Mvc;

namespace SignMeUp2.ViewModels
{
    public enum FaltTyp { text_falt = 0, val_falt = 1 }

    public class FaltViewModel
    {
        //public FaltViewModel()
        //{
        //    Ovrigt = new Dictionary<string, object>();
        //}

        public string Namn { get; set; }

        //public string Rubrik { get; set; }

        public string Varde { get; set; }

        public bool Kravs { get; set; }

        public string Valideringsmeddelande { get { return Namn + " måste anges"; } }

        public SelectList Alternativ { get; set; }

        public FaltTyp Typ { get; set; }

        //public Dictionary<string, object> Ovrigt { get; private set; }

        //public void LaggTillOvrigt(string namn, object varde)
        //{
        //    Ovrigt[namn] = varde;
        //}

        //public object HamtaOvrigt(string namn)
        //{
        //    return Ovrigt[namn];
        //}
    }
}