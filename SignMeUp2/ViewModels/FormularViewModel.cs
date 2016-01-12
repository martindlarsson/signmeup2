using System.Collections.Generic;
using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    public class FormularViewModel
    {
        public int Id { get; set; }

        public int? EvenemangsId { get; set; }
        public Evenemang Evenemang { get; set; }
        
        public string Namn { get; set; }
        
        public bool Gratis { get; set; }
        
        public int Avgift { get; set; }

        public virtual List<FormularSteg> Steg { get; set; }
    }
}