namespace SignMeUp2.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum TypAvgift { Avgift = 0, Rabatt = 1}

    [Table("Forseningsavgift")]
    public partial class Forseningsavgift
    {
        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        public virtual Evenemang Evenemang { get; set; }
        public int? EvenemangsId { get; set; }

        [Required]
        public DateTime FranDatum { get; set; }

        [Required]
        public DateTime TillDatum { get; set; }

        [Required]
        public TypAvgift PlusEllerMinus { get; set; }

        [Required]
        public int Summa { get; set; }
    }
}
