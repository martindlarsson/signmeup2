namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Rabatter")]
    public partial class Rabatter
    {
        public int Id { get; set; }

        [Required]
        public string Kod { get; set; }

        [Required]
        public int Summa { get; set; }

        public string Beskrivning { get; set; }

        public virtual Evenemang Evenemang { get; set; }
        public int EvenemangsId { get; set; }
    }
}
