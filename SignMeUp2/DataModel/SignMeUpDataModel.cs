namespace SignMeUp2.DataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SignMeUpDataModel : DbContext
    {
        public SignMeUpDataModel()
            : base("name=SignMeUpDataModel")
        {
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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
                .HasForeignKey(e => e.Evenemang_ID)
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
                .WillCascadeOnDelete(false);
        }
    }
}
