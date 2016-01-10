namespace SignMeUp2.Data
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public partial class SignMeUpDataModel : IdentityDbContext<ApplicationUser>
    {
        public SignMeUpDataModel()
            : base("SignMeUpDataModel", throwIfV1Schema: false)
        {
        }

        public static SignMeUpDataModel Create() {
            return new SignMeUpDataModel();
        }

        //public virtual DbSet<Banor> Banor { get; set; }
        //public virtual DbSet<Deltagare> Deltagare { get; set; }
        public virtual DbSet<Evenemang> Evenemang { get; set; }
        public virtual DbSet<Forseningsavgift> Forseningsavgift { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        //public virtual DbSet<Kanoter> Kanoter { get; set; }
        //public virtual DbSet<Klasser> Klasser { get; set; }
        public virtual DbSet<Rabatter> Rabatter { get; set; }
        public virtual DbSet<Registreringar> Registreringar { get; set; }
        public virtual DbSet<Organisation> Organisationer { get; set; }
        public virtual DbSet<Betalningsmetoder> Betalningsmetoders { get; set; }

        public virtual DbSet<Formular> Formular { get; set; }
        public virtual DbSet<WizardStep> Steg { get; set; }
        public virtual DbSet<Falt> Falt { get; set; }
        public virtual DbSet<Val> Val { get; set; }
        public virtual DbSet<FaltSvar> Svar { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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

            //modelBuilder.Entity<Evenemang>()
            //    .HasMany(e => e.Registreringar)
            //    .WithRequired(r => r.Evenemang)
            //    .HasForeignKey(r => r.EvenemangsId)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Formular)
                .WithRequired(f => f.Evenemang)
                .HasForeignKey(f => f.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Formular>()
                .HasMany(f => f.Steg)
                .WithRequired(s => s.Formular)
                .HasForeignKey(s => s.FormularsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<WizardStep>()
                .HasMany(s => s.Falt)
                .WithRequired(f => f.Steg)
                .HasForeignKey(f => f.StegId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Falt>()
                .HasMany(f => f.Val)
                .WithRequired(v => v.Falt)
                .HasForeignKey(v => v.FaltId)
                .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Evenemang>()
            //    .HasMany(e => e.Banor)
            //    .WithRequired(e => e.Evenemang)
            //    .HasForeignKey(e => e.EvenemangsId)
            //    .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Evenemang>()
            //    .HasMany(e => e.Klasser)
            //    .WithRequired(e => e.Evenemang)
            //    .HasForeignKey(e => e.EvenemangsId)
            //    .WillCascadeOnDelete(true);

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

            //modelBuilder.Entity<Evenemang>()
            //    .HasMany(e => e.Kanoter)
            //    .WithRequired(e => e.Evenemang)
            //    .HasForeignKey(e => e.EvenemangsId)
            //    .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Registreringar>()
            //    .HasRequired(r => r.Bana)
            //    .WithMany(b => b.Registreringar)
            //    .HasForeignKey(r => r.Bana_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Registreringar>()
            //    .HasRequired(r => r.Kanot)
            //    .WithMany(k => k.Registreringar)
            //    .HasForeignKey(r => r.Kanot_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Registreringar>()
            //    .HasRequired(r => r.Klass)
            //    .WithMany(k => k.Registreringar)
            //    .HasForeignKey(r => r.Klass_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Registreringar>()
            //    .HasMany(e => e.Deltagare)
            //    .WithRequired(d => d.Registreringar)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<Registreringar>()
                .HasMany(r => r.Svar)
                .WithRequired(s => s.Registrering)
                .HasForeignKey(s => s.RegistreringsId)
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
