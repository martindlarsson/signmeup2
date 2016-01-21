namespace SignMeUp2.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Aktiviteter")]
    public class Aktivitet
    {
        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }
        
        public virtual ICollection<Formular> Formular { get; set; }
    }
}
