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
        
        public virtual DbSet<Evenemang> Evenemang { get; set; }
        public virtual DbSet<Forseningsavgift> Forseningsavgift { get; set; }
        public virtual DbSet<Fakturaadress> Invoice { get; set; }
        public virtual DbSet<Rabatter> Rabatter { get; set; }
        public virtual DbSet<Registrering> Registreringar { get; set; }
        public virtual DbSet<Organisation> Organisationer { get; set; }
        public virtual DbSet<Betalningsmetoder> Betalningsmetoders { get; set; }

        public virtual DbSet<Formular> Formular { get; set; }
        public virtual DbSet<FormularSteg> Steg { get; set; }
        public virtual DbSet<Falt> Falt { get; set; }
        public virtual DbSet<Val> Val { get; set; }
        public virtual DbSet<FaltSvar> Svar { get; set; }
        public virtual DbSet<Lista> Listor { get; set; }
        public virtual DbSet<ListaFalt> ListFalt { get; set; }

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

            //modelBuilder.Entity<Evenemang>()
            //    .HasMany(e => e.Listor)
            //    .WithRequired(l => l.Evenemang)
            //    .HasForeignKey(l => l.EvenemangId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Lista>()
            //    .HasRequired(l => l.Formular)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Formular>()
                .HasMany(f => f.Listor)
                .WithRequired(l => l.Formular)
                .HasForeignKey(l => l.FormularId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Lista>()
                .HasMany(l => l.Falt) // ListaFalt mappar falten i listan till falten
                .WithRequired(lf => lf.Lista)
                .HasForeignKey(lf => lf.ListaId)
                .WillCascadeOnDelete(false); // Tar inte bort faltet om listan tas bort

            modelBuilder.Entity<Falt>()
                .HasMany(f => f.Listor) // ListaFalt mappar falten i listan till falten
                .WithRequired(lf => lf.Falt)
                .HasForeignKey(lf => lf.FaltId)
                .WillCascadeOnDelete(true); // Tar bort faltet ur listan om faltet tas bort

            modelBuilder.Entity<Formular>()
                .HasMany(f => f.Registreringar)
                .WithRequired(r => r.Formular)
                .HasForeignKey(r => r.FormularId);

            modelBuilder.Entity<Formular>()
                .HasMany(f => f.Steg)
                .WithRequired(s => s.Formular)
                .HasForeignKey(s => s.FormularId);
            
            modelBuilder.Entity<FormularSteg>()
                .HasMany(s => s.Falt)
                .WithRequired(f => f.Steg)
                .HasForeignKey(f => f.StegId);

            modelBuilder.Entity<Falt>()
                .HasMany(f => f.Val)
                .WithRequired(v => v.Falt)
                .HasForeignKey(v => v.FaltId);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Rabatter)
                .WithRequired(r => r.Evenemang)
                .HasForeignKey(r => r.EvenemangsId);

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
        }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
