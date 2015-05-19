using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.DataModel;

namespace SignMeUp2.Models
{
    public class StartlistaViewModel
    {
        public StartlistaViewModel() { BanLista = new List<RegBanorViewModel>(); }

        public string Evenemang { get; set; }
        public int AntalAnmälda
        {
            get
            {
                int sum = 0;
                foreach (var banReg in BanLista)
                {
                    sum += banReg.AntalAnmälda;
                }
                return sum;
            }
        }
        public IList<RegBanorViewModel> BanLista { get; set; }

    public class RegBanorViewModel
    {
        public RegBanorViewModel() { KlassLista = new List<RegKlassViewModel>(); }
        public string Namn { get; set; }
        public IList<RegKlassViewModel> KlassLista { get; set; }
        public int AntalAnmälda
        {
            get
            {
                int sum = 0;
                foreach (var regList in KlassLista)
                {
                    sum += regList.AntalAnmälda;
                }
                return sum;
            }
        }
    }

    public class RegKlassViewModel
    {
        public RegKlassViewModel() { RegistreringarList = new List<RegistreringViewModel>(); }
        public string Namn { get; set; }
        public int AntalAnmälda { get { return RegistreringarList.Count(); } }
        public IList<RegistreringViewModel> RegistreringarList { get; set; }
    }

    public class RegistreringViewModel
    {
        public RegistreringViewModel() { DeltagarLista = new List<DeltagareSimple>(); }
        public string Lagnamn { get; set; }
        public IList<DeltagareSimple> DeltagarLista { get; set; }
        public string Klubb { get; set; }
    }

    public class DeltagareSimple
    {
        public string Namn { get; set; }
    }

    /// <summary>
    /// Skapa en startlista utifrån en lista på registreringar
    /// </summary>
    /// <param name="registreringar"></param>
    /// <returns></returns>
    public static StartlistaViewModel GetStartlist(IList<Registreringar> registreringar, string Evenemang, IList<Banor> banor, IList<Klasser> klasser)
    {
        var regList = new StartlistaViewModel();
        regList.Evenemang = Evenemang;

        // Skapa lista på banor
        foreach (var bana in banor)
        {
            var banReg = new RegBanorViewModel { Namn = bana.Namn };
            regList.BanLista.Add(banReg);

            foreach (var klass in klasser)
            {
                banReg.KlassLista.Add(new RegKlassViewModel { Namn = klass.Namn });
            }
        }

        // Iterera över alla registreringar och mappa objekten samt stoppa in dem i rätt lista
        foreach (var reg in registreringar)
        {
            // Mappa registreringen
            var registrering = new RegistreringViewModel
            {
                Lagnamn = reg.Lagnamn,
                DeltagarLista = new List<DeltagareSimple>(),
                Klubb = reg.Klubb
            };

            // Mappa deltagarna
            foreach (var deltagare in reg.Deltagare)
            {
                var deltagareViewModel = new DeltagareSimple
                {
                    Namn = string.Format("{0} {1}", deltagare.Förnamn, deltagare.Efternamn)
                };
                registrering.DeltagarLista.Add(deltagareViewModel);
            }

            // Hitta rätt lista att stoppa in registreringen i
            var banList = regList.BanLista.FirstOrDefault(banReg => banReg.Namn == reg.Bana.Namn);
            var klassList = banList.KlassLista.FirstOrDefault(klassL => klassL.Namn == reg.Klass.Namn);
            klassList.RegistreringarList.Add(registrering);
        }

        return regList;
    }
    }
}