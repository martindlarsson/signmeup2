namespace SignMeUp2.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Invoice")]
    public partial class Fakturaadress
    {
        public int Id { get; set; }

        [Required]
        public string Postnummer { get; set; }

        [Required]
        public string Organisationsnummer { get; set; }

        [Required]
        public string Postort { get; set; }

        [Required]
        public string Postadress { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public string Epost { get; set; }

        public Registrering Registrering { get; set; }
    }
}
