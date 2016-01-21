using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Data
{
    [Table("Formular")]
    public partial class Formular
    {
        public Formular()
        {
            Registreringar = new List<Registrering>();
            Steg = new List<FormularSteg>();
        }
        
        public int Id { get; set; }

        public int? EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }
        
        [Required]
        public string Namn { get; set; }

        [Required]
        public int Avgift { get; set; }

        public int? MaxRegistreringar { get; set; }

        [Required]
        public bool Publikt { get; set; }

        public int AktivitetsId { get; set; }

        public Aktivitet Aktivitet { get; set; }
        
        public virtual ICollection<Registrering> Registreringar { get; set; }

        public virtual ICollection<FormularSteg> Steg { get; set; }

        public virtual ICollection<Lista> Listor { get; set; }
    }
}
