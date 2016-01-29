using System;
using System.Collections.Generic;

namespace SignMeUp2.ViewModels
{   
    public class RegistreringVM
    {
        public RegistreringVM()
        {
            Svar = new List<FaltSvarVM>();
        }

        public int Id { get; set; }

        public int? FormularId { get; set; }
        public virtual FormularViewModel Formular { get; set; }

        public virtual ICollection<FaltSvarVM> Svar { get; set; }
        
        public bool HarBetalt { get; set; }

        public string Kommentar { get; set; }

        public string PaysonToken { get; set; }
        
        public DateTime? Registreringstid { get; set; }

        public int AttBetala { get; set; }

        public int Forseningsavgift { get; set; }

        public int Rabatt { get; set; }
        public string Rabattkod { get; set; }

        public InvoiceViewModel Invoice { get; set; }
    }
}
