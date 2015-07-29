namespace SignMeUp2.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Kanoter")]
    public partial class Kanoter
    {
        public Kanoter()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public int Avgift { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }
}
