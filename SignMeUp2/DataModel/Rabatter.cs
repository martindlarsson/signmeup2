namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Rabatter")]
    public partial class Rabatter
    {
        public int Id { get; set; }

        [Required]
        public string Kod { get; set; }

        public int Summa { get; set; }

        [Required]
        public string Beskrivning { get; set; }

        public int Evenemang_ID { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }
}
