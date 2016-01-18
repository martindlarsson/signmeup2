using System.Collections.Generic;

namespace SignMeUp2.ViewModels
{
    public class TabellViewModel
    {
        public TabellViewModel()
        {
            Kolumner = new List<Kolumn>();
            Rader = new List<Rad>();
        }

        public string Namn { get; set; }
        public ICollection<Kolumn> Kolumner { get; set; }
        public ICollection<Rad> Rader { get; set; }
    }

    public class Kolumn
    {
        public string Rubrik { get; set; }
        public int FaltId { get; set; }
    }

    public class Rad
    {
        public Rad()
        {
            Varden = new List<string>();
        }

        public ICollection<string> Varden { get; set; }
    }
}