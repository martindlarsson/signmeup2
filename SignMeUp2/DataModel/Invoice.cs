namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

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

        public Registreringar Registrering { get; set; }
    }

    public class InvoiceMap : EntityTypeConfiguration<Invoice>
    {
        public InvoiceMap()
        {
            // Key
            HasKey(i => i.Id);

            // Properties
            Property(i => i.Box).IsOptional();
            Property(i => i.Postnummer).IsRequired();
            Property(i => i.Organisationsnummer).IsRequired();
            Property(i => i.Postort).IsRequired();
            Property(i => i.Postadress).IsRequired();
            Property(i => i.Namn).IsRequired();
            Property(i => i.Att).IsRequired();

            // Relatiionship
            HasRequired(i => i.Registrering)
                .WithRequiredDependent(r => r.Invoice)
                .WillCascadeOnDelete(true);
        }
    }
}
