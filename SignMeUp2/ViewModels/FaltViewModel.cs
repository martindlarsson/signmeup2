using System.Collections.Generic;
using System.Web.Mvc;
using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
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