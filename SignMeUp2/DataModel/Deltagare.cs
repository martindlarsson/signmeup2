namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Deltagare")]
    public partial class Deltagare
    {
        public int Id { get; set; }

        [Required]
        public string Förnamn { get; set; }

        [Required]
        public string Efternamn { get; set; }

        public string Personnummer { get; set; }

        public int RegistreringarID { get; set; }

        public virtual Registreringar Registreringar { get; set; }
    }
}
