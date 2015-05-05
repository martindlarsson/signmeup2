namespace SignMeUp2.DataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    //using Microsoft.AspNet.Identity;
    //using Microsoft.AspNet.Identity.EntityFramework;
    //using SignMeUp2.Models;

    public partial class SignMeUpDataModel : DbContext
    {
        public SignMeUpDataModel()
            : base("name=SignMeUpDataModel")
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // TODO orgBetalningar

            // TODO invoce cascade delete

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Evenemang)
                .WithRequired(e => e.Organisation)
                .HasForeignKey(e => e.OrganisationsId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Banor>()
                .HasMany(e => e.Registreringar)
                .WithRequired(e => e.Banor)
                .HasForeignKey(e => e.Bana)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Registreringar)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.Evenemang_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Banor)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.EvenemangsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Klasser)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.Evenemang_ID);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Rabatter)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.Evenemang_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evenemang>()
                .HasMany(e => e.Kanoter)
                .WithRequired(e => e.Evenemang)
                .HasForeignKey(e => e.Evenemang_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kanoter>()
                .HasMany(e => e.Registreringar)
                .WithRequired(e => e.Kanoter)
                .HasForeignKey(e => e.Kanot)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Klasser>()
                .HasMany(e => e.Registreringar)
                .WithRequired(e => e.Klasser)
                .HasForeignKey(e => e.Klass)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registreringar>()
                .HasMany(e => e.Deltagare)
                .WithRequired(e => e.Registreringar)
                .WillCascadeOnDelete(true);
        }

        public System.Data.Entity.DbSet<SignMeUp2.DataModel.Betalningsmetoder> Betalningsmetoders { get; set; }
    }
}
