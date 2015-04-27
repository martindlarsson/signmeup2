using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.DataModel;

namespace SignMeUp2.Models
{
    public class StartlistaViewModel
    {
        public int AntalAnmälda
        {
            get
            {
                int sum = 0;
                foreach (var banReg in BanorRegistreringar)
                {
                    sum += banReg.AntalAnmälda;
                }
                return sum;
            }
        }
        public IList<RegBanorViewModel> BanorRegistreringar { get; set; }

        /// <summary>
        /// Skapa en startlista utifrån en lista på registreringar
        /// </summary>
        /// <param name="registreringar"></param>
        /// <returns></returns>
        public static StartlistaViewModel GetStartlist(IList<Registreringar> registreringar, IList<Banor> banor, IList<Klasser> klasser) {
            var regList = new StartlistaViewModel();

            // Skapa lista på banor
            regList.BanorRegistreringar = new List<RegBanorViewModel>();
            foreach (var bana in banor)
            {
                var banReg = new RegBanorViewModel { Namn = bana.Namn };
                regList.BanorRegistreringar.Add(banReg);

                banReg.regKlassList = new List<RegKlassViewModel>();
                foreach (var klass in klasser)
                {
                    banReg.regKlassList.Add(new RegKlassViewModel { Namn = klass.Namn });
                }
            }

            // Iterera över alla registreringar och mappa objekten samt stoppa in dem i rätt lista
            foreach (var reg in registreringar)
            {
                // Mappa registreringen
                var registrering = new RegistreringViewModel
                {
                    Lagnamn = reg.Lagnamn,
                    DeltagarLista = new List<DeltagareViewModel>()
                };

                // Mappa deltagarna
                foreach (var deltagare in reg.Deltagare)
                {
                    var deltagareViewModel = new DeltagareViewModel
                    {
                        Förnamn = deltagare.Förnamn,
                        Efternamn = deltagare.Efternamn
                    };
                    registrering.DeltagarLista.Add(deltagareViewModel);
                }

                // Hitta rätt lista att stoppa in registreringen i
                var banList = regList.BanorRegistreringar.FirstOrDefault(banReg => banReg.Namn == reg.Banor.Namn);
                var klassList = banList.regKlassList.FirstOrDefault(klassL => klassL.Namn == reg.Klasser.Namn);
                klassList.regList.Add(registrering);
            }

            return regList;
        }
    }

    public class RegBanorViewModel
    {
        public string Namn { get; set; }
        public IList<RegKlassViewModel> regKlassList { get; set; }
        public int AntalAnmälda
        {
            get
            {
                int sum = 0;
                foreach (var regList in regKlassList)
                {
                    sum += regList.AntalAnmälda;
                }
                return sum;
            }
        }
    }

    public class RegKlassViewModel
    {
        public string Namn { get; set; }
        public int AntalAnmälda { get { return regList.Count(); } }
        public IList<RegistreringViewModel> regList { get; set; }
    }

    public class RegistreringViewModel
    {
        public string Lagnamn { get; set; }
        public IList<DeltagareViewModel> DeltagarLista { get; set; }
    }
}