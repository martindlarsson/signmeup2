namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invoice")]
    public partial class Invoice
    {
        public int Id { get; set; }

        public string Box { get; set; }

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

        public string Att { get; set; }

        //public int Registreringar_ID { get; set; }

        //public virtual Registreringar Registreringar { get; set; }
    }
}
