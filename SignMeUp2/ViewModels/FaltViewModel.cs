using System.Collections.Generic;
using System.Web.Mvc;
using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    //public enum FaltTyp { text_falt = 0, val_falt = 1, epost_falt = 3 }

    public class FaltViewModel
    {
        public string Namn { get; set; }

        public string Varde { get; set; }

        public bool Kravs { get; set; }

        public IList<ValViewModel> Val { get; set; }

        public SelectList Alternativ
        {
            get
            {
                return new SelectList(Val, "Id", "Namn", Varde);
            }
        }

        public FaltTyp Typ { get; set; }

        public bool Avgiftsbelagd { get; set; }

        public int FaltId { get; set; }
    }
}