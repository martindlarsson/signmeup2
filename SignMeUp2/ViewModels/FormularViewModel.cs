using System.Collections.Generic;
using SignMeUp2.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SignMeUp2.ViewModels
{
    public class FormularViewModel
    {
        public FormularViewModel()
        {
            //Registreringar = new List<RegistreringVM>();
            Steg = new List<FormularStegVM>();
        }

        public int Id { get; set; }

        public int? EvenemangsId { get; set; }

        public EvenemangVM Evenemang { get; internal set; }

        [Required(ErrorMessage = "Namn måste anges")]
        public string Namn { get; set; }
        
        [Required(ErrorMessage = "Avgift måste anges")]
        public int Avgift { get; set; }

        public int? MaxRegistreringar { get; set; }
        
        public bool Publikt { get; set; }

        [Required(ErrorMessage = "Aktivitet måste anges")]
        public int AktivitetsId { get; set; }

        public AktivitetViewModel Aktivitet { get; set; }

        public SelectList Aktiviteter { get; set; }

        public string AnnanAktivitet { get; set; }

        //public ICollection<RegistreringVM> Registreringar { get; set; }

        public ICollection<FormularStegVM> Steg { get; set; }

        public ICollection<Lista> Listor { get; set; }
    }
}