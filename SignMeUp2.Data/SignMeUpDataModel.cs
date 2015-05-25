namespace SignMeUp2.Data
{
    using System;
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SignMeUpDataModel : IdentityDbContext<ApplicationUser>
    {
        public SignMeUpDataModel()
            //: base("name=SignMeUpDataModel")
            : base("SignMeUpDataModel", throwIfV1Schema: false)
        {
        }

        public static SignMeUpDataModel Create() {
            return new SignMeUpDataModel();
        }

        public virtual DbSet<Banor> Banor { get; set; }
        public virtual DbSet<Deltagare> Deltagare { get; set; }
        public virtual DbSet<Evenemang> Evenemang { get; set; }
        public virtual DbSet<Forseningsavgift> Forseningsavgift { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<Kanoter> Kanoter { get; set; }
        public virtual DbSet<Klasser> Klasser { get; set; }
        public virtual DbSet<Rabatter> Rabatter { get; set; }
        public virtual DbSet<Registreringar> Registreringar { get; set; }
        public virtual DbSet<Organisation> Organisationer { get; set; }
        public virtual DbSet<Betalningsmetoder> Betalningsmetoders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new EvenemangMap());
            //modelBuilder.Configurations.Add(new ForseningsavgiftMap());
            //modelBuilder.Configurations.Add(new BanorMap());
            //modelBuilder.Configurations.Add(new KlasserMap());
            //modelBuilder.Configurations.Add(new KanoterMap());
            //modelBuilder.Configurations.Add(new RegistreringarMap());
            //modelBuilder.Configurations.Add(new DeltagareMap());
            //modelBuilder.Configurations.Add(new BetalningsmetoderMap());
            //modelBuilder.Configurations.Add(new InvoiceMap());
            //modelBuilder.Configurations.Add(new OrgMap());
            //modelBuilder.Configurations.Add(new RabattMap());

            // TODO orgBetalningar

            // Identity context model creation
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organisation>()
                .HasOptional(o => o.Betalningsmetoder)
                .WithRequired(b => b.Organisation)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Evenemang)
                .WithRequired(e => e.Organisation)
                .HasForeignKey(e => e.OrganisationsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Registreringar)
                .WithRequired(r => r.Evenemang)
                .HasForeignKey(r => r.EvenemangsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Banor)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Klasser)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Rabatter)
                .WithRequired(r => r.Evenemang)
                .HasForeignKey(r => r.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Forseningsavgifter)
                .WithRequired(f => f.Evenemang)
                .HasForeignKey(f => f.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Kanoter)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Registreringar>()
                .HasRequired(r => r.Bana)
                .WithMany(b => b.Registreringar)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registreringar>()
                .HasRequired(r => r.Kanot)
                .WithMany(k => k.Registreringar)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registreringar>()
                .HasRequired(r => r.Klass)
                .WithMany(k => k.Registreringar)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registreringar>()
                .HasMany(e => e.Deltagare)
                .WithRequired(d => d.Registreringar)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Registreringar>()
                .HasOptional(r => r.Invoice)
                .WithRequired(i => i.Registrering)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Registreringar>()
                .HasOptional(r => r.Forseningsavgift)
                .WithMany()
                .HasForeignKey(r => r.ForseningsavgiftId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registreringar>()
                .HasOptional(r => r.Rabatt)
                .WithMany()
                .HasForeignKey(r => r.RabattId)
                .WillCascadeOnDelete(false);
        }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
