namespace SignMeUp2.DataModel
{
    using System;
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using SignMeUp2.Models;

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
            modelBuilder.Configurations.Add(new EvenemangMap());
            modelBuilder.Configurations.Add(new ForseningsavgiftMap());
            modelBuilder.Configurations.Add(new BanorMap());
            modelBuilder.Configurations.Add(new KlasserMap());
            modelBuilder.Configurations.Add(new KanoterMap());
            modelBuilder.Configurations.Add(new RegistreringarMap());
            modelBuilder.Configurations.Add(new DeltagareMap());
            modelBuilder.Configurations.Add(new BetalningsmetoderMap());
            modelBuilder.Configurations.Add(new InvoiceMap());
            modelBuilder.Configurations.Add(new OrgMap());
            modelBuilder.Configurations.Add(new RabattMap());

            // TODO orgBetalningar

            // Identity context model creation
            base.OnModelCreating(modelBuilder);
        }
    }
}
