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

        [Required(ErrorMessage = "Postnummer m�ste anges")]
        public string Postnummer { get; set; }

        [Required(ErrorMessage = "Organisationsnummer m�ste anges")]
        public string Organisationsnummer { get; set; }

        [Required(ErrorMessage = "Postort m�ste anges")]
        public string Postort { get; set; }

        [Required(ErrorMessage = "Postadress m�ste anges")]
        public string Postadress { get; set; }

        [Required(ErrorMessage = "F�retagsnamn m�ste anges")]
        [Display(Name = "F�retagsnamn")]
        public string Namn { get; set; }

        public string Att { get; set; }

        //public int Registreringar_ID { get; set; }

        //public virtual Registreringar Registreringar { get; set; }
    }
}
