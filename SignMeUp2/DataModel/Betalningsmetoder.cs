namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    public enum GiroTyp { Inget = 0, Postgiro = 1, Bankgiro = 2 }

    [Table("Betalningsmetoder")]
    public class Betalningsmetoder
    {
        public int Id { get; set; }

        public GiroTyp GiroTyp { get; set; }

        public string Gironummer { get; set; }

        public string PaysonUserId { get; set; }

        public string PaysonUserKey { get; set; }

        public Organisation Organisation { get; set; }
    }

    public class BetalningsmetoderMap : EntityTypeConfiguration<Betalningsmetoder>
    {
        public BetalningsmetoderMap()
        {
            // Key
            HasKey(b => b.Id);

            // Properties
            Property(b => b.GiroTyp).IsRequired();
            Property(b => b.Gironummer).IsOptional();
            Property(b => b.PaysonUserId).IsOptional();
            Property(b => b.PaysonUserKey).IsOptional();

            // Relatiionship
            HasRequired(b => b.Organisation)
                .WithRequiredDependent(o => o.Betalningsmetoder);
        }
    }
}