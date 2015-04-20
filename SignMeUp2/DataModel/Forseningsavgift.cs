namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Forseningsavgift")]
    public partial class Forseningsavgift
    {
        public int ID { get; set; }

        public DateTime FranDatum { get; set; }

        public int Summa { get; set; }
    }
}
