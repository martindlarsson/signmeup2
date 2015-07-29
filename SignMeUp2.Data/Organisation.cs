namespace SignMeUp2.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Organisationer")]
    public class Organisation
    {
        public Organisation()
        {
            Evenemang = new HashSet<Evenemang>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public string Epost { get; set; }

        [Required]
        public string Adress { get; set; }

        // Bild för logo på t.ex. fakturor eller i formuläret
        public string BildUrl { get; set; }

        public virtual ICollection<Evenemang> Evenemang { get; set; }

        public virtual Betalningsmetoder Betalningsmetoder { get; set; }
        
        public string AnvändareId { get; set; }
    }
}