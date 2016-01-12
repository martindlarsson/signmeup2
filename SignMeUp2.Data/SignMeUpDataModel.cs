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
        public virtual DbSet<Fakturaadress> Invoice { get; set; }
        //public virtual DbSet<Kanoter> Kanoter { get; set; }
        //public virtual DbSet<Klasser> Klasser { get; set; }
        public virtual DbSet<Rabatter> Rabatter { get; set; }
        public virtual DbSet<Registrering> Registreringar { get; set; }
        public virtual DbSet<Organisation> Organisationer { get; set; }
        public virtual DbSet<Betalningsmetoder> Betalningsmetoders { get; set; }

        public virtual DbSet<Formular> Formular { get; set; }
        public virtual DbSet<FormularSteg> Steg { get; set; }
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

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Formular)
                .WithRequired(f => f.Evenemang)
                .HasForeignKey(f => f.EvenemangsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Formular>()
                .HasMany(f => f.Registreringar)
                .WithRequired(r => r.Formular)
                .HasForeignKey(r => r.FormularId);
            //.WillCascadeOnDelete(true);

            modelBuilder.Entity<Formular>()
                .HasMany(f => f.Steg)
                .WithRequired(s => s.Formular)
                .HasForeignKey(s => s.FormularId);
            //.WillCascadeOnDelete(true);

            //modelBuilder.Entity<FormularSteg>()
            //    .HasRequired(f => f.Formular)
            //    .WithMany(f => f.Steg)
            //    .HasForeignKey(f => f.FormularsId)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<FormularSteg>()
                .HasMany(s => s.Falt)
                .WithRequired(f => f.Steg)
                .HasForeignKey(f => f.StegId);
            //.WillCascadeOnDelete(true);

            //modelBuilder.Entity<Falt>()
            //    .HasRequired(f => f.Steg)
            //    .WithMany(s => s.Falt)
            //    .HasForeignKey(f => f.StegId)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<Falt>()
                .HasMany(f => f.Val)
                .WithRequired(v => v.Falt)
                .HasForeignKey(v => v.FaltId);
            //.WillCascadeOnDelete(true);

            //modelBuilder.Entity<Val>()
            //    .HasRequired(v => v.Falt)
            //    .WithMany(f => f.Val)
            //    .HasForeignKey(v => v.FaltId)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Rabatter)
                .WithRequired(r => r.Evenemang)
                .HasForeignKey(r => r.EvenemangsId);
                //.WillCascadeOnDelete(true);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Forseningsavgifter)
                .WithRequired(f => f.Evenemang)
                .HasForeignKey(f => f.EvenemangsId)
                .WillCascadeOnDelete(true);
            
            modelBuilder.Entity<Registrering>()
                .HasMany(r => r.Svar)
                .WithRequired(s => s.Registrering)
                .HasForeignKey(s => s.RegistreringsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<FaltSvar>()
                .HasRequired(fs => fs.Falt)
                .WithMany()
                .HasForeignKey(fs => fs.FaltId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registrering>()
                .HasOptional(r => r.Invoice)
                .WithRequired(i => i.Registrering)
                .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Registrering>()
            //    .HasOptional(r => r.Forseningsavgift)
            //    .WithMany()
            //    .HasForeignKey(r => r.Forseningsavgift)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Registrering>()
            //    .HasOptional(r => r.Rabatt)
            //    .WithMany()
            //    .HasForeignKey(r => r.Rabatt)
            //    .WillCascadeOnDelete(false);
        }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
