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

        [Required(ErrorMessage = "Postnummer måste anges")]
        public string Postnummer { get; set; }

        [Required(ErrorMessage = "Organisationsnummer måste anges")]
        public string Organisationsnummer { get; set; }

        [Required(ErrorMessage = "Postort måste anges")]
        public string Postort { get; set; }

        [Required(ErrorMessage = "Postadress måste anges")]
        public string Postadress { get; set; }

        [Required(ErrorMessage = "Företagsnamn måste anges")]
        [Display(Name = "Företagsnamn")]
        public string Namn { get; set; }

        public string Att { get; set; }

        //public int Registreringar_ID { get; set; }

        //public virtual Registreringar Registreringar { get; set; }
    }
}
