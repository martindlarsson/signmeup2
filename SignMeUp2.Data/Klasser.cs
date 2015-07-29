namespace SignMeUp2.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Klasser")]
    public partial class Klasser
    {
        public Klasser()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }
}
