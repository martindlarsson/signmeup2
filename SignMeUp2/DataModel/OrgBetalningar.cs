//namespace SignMeUp2.DataModel
//{
//    using System;
//    using System.Collections.Generic;
//    using System.ComponentModel.DataAnnotations;
//    using System.ComponentModel.DataAnnotations.Schema;
//    using System.Data.Entity.Spatial;

//    [Table("OrgBetalningar")]
//    public class OrgBetalningar
//    {
//        public OrgBetalningar()
//        {
//        }

//        public int ID { get; set; }

//        [Required]
//        public DateTime Inbetalningstid { get; set; }

//        [Required]
//        public int Summa { get; set; }

//        public string Spårningssträng { get; set; }
//    }
//}