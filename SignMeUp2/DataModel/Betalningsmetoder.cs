namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public enum GiroTyp { Inget = 0, Postgiro = 1, Bankgiro = 2 }

    [Table("Betalningsmetoder")]
    public class Betalningsmetoder
    {
        public Betalningsmetoder()
        {
        }

        public int ID { get; set; }

        public GiroTyp GiroTyp { get; set; }

        public string Gironummer { get; set; }

        public string PaysonUserId { get; set; }

        public string PaysonUserKey { get; set; }
    }
}