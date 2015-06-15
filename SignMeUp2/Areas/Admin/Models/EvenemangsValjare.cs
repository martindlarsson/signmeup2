using SignMeUp2.Data;
using System.Web.Mvc;

namespace SignMeUp2.Areas.Admin.Models
{
    public class EvenemangsValjare
    {
        public SelectList Evenemang { get; set; }
        public int SelectedEvenemangsId { get; set; }
        public string SelectedEvenemangsNamn { get; set; }
    }
}