namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

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

    public class DeltagareMap : EntityTypeConfiguration<Deltagare>
    {
        public DeltagareMap()
        {
            // Key
            HasKey(d => d.Id);

            // Properties
            Property(d => d.Förnamn).IsRequired();
            Property(d => d.Efternamn).IsRequired();
            Property(d => d.Personnummer).IsOptional();

            // Relatiionship
            HasRequired(d => d.Registreringar)
                .WithMany(r => r.Deltagare)
                .HasForeignKey(d => d.RegistreringarID)
                .WillCascadeOnDelete(true);
        }
    }
}
