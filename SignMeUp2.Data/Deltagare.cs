namespace SignMeUp2.Data
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Deltagare")]
    public partial class Deltagare
    {
        public int Id { get; set; }

        public string Förnamn { get; set; }

        public string Efternamn { get; set; }

        public string Personnummer { get; set; }

        public int RegistreringarID { get; set; }
        public virtual Registreringar Registreringar { get; set; }
    }
}
