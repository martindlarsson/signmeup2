using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class DeltagareViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Förnamn måste anges")]
        public string Förnamn { get; set; }

        [Required(ErrorMessage = "Efternamn måste anges")]
        public string Efternamn { get; set; }
    }

    public class DeltagareListViewModel : FormularSteg
    {
        public IList<DeltagareViewModel> DeltagareLista { get; set; }

        private int _AntalDeltagareBana;
        public int AntalDeltagareBana
        {
            get { return _AntalDeltagareBana; }
            set
            {
                _AntalDeltagareBana = value;
                if (DeltagareLista == null || DeltagareLista.Count() != _AntalDeltagareBana)
                {
                    DeltagareLista = new List<DeltagareViewModel>();
                    for (int i = 0; i < _AntalDeltagareBana; i++)
                    {
                        DeltagareLista.Add(new DeltagareViewModel());
                    }
                }
            }
        }
    }
}