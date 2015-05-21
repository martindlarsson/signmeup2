using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.Data;
using SignMeUp2.Models;
using log4net;

namespace SignMeUp2.Services
{
    public class SignMeUpService : IDisposable
    {
        protected readonly ILog log;
        private SignMeUpDataModel _context;

        private Evenemang CurrentEvenemang { get; set; }

        // Singleton
        private static SignMeUpService instance;
        public static SignMeUpService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SignMeUpService();
                }
                return instance;
            }
        }

        private SignMeUpService()
        {
            log = LogManager.GetLogger(GetType());
        }

        public SignMeUpDataModel Db
        {
            get
            {
                if (_context == null)
                {
                    _context = new SignMeUpDataModel();
                }
                return _context;
            }
        }


        public IList<Registreringar> GetRegistreringar(int evenemangsId)
        {
            return Db.Registreringar.Where(reg => reg.EvenemangsId == evenemangsId).ToList();
        }

        public IList<Registreringar> GetRegistreringar()
        {
            return Db.Registreringar.ToList();
        }

        public Registreringar GetRegistrering(int id, bool fill)
        {
            var reg = Db.Registreringar.Find(id);

            return fill ? FillRegistrering(reg) : reg;
        }

        public Registreringar FillRegistrering(Registreringar reg)
        {
            //reg.Kanot = Db.Kanoter.Find(reg.KanotId);
            //reg.Bana = Db.Banor.Find(reg.BanId);
            //reg.Klass = Db.Klasser.Find(reg.KlassId);
            reg.Evenemang = Db.Evenemang.Find(reg.EvenemangsId);
            reg.Evenemang.Organisation = Db.Organisationer.Find(reg.Evenemang.OrganisationsId);

            //if (reg.Rabatt == null && reg.RabattId != 0)
            //{
            //    reg.Rabatt = Db.Rabatter.Find(reg.RabattId);
            //}

            //if (reg.Forseningsavgift == null && reg.ForseningsavgiftsId != 0)
            //{
            //    reg.Forseningsavgift = Db.Forseningsavgift.Find(reg.ForseningsavgiftsId);
            //}

            return reg;
        }

        public void SparaNyRegistrering(Registreringar reg)
        {
            try
            {
                reg.Registreringstid = DateTime.Now;

                if (reg.Bana != null)
                {
                    Db.Banor.Attach(reg.Bana);
                }

                if (reg.Kanot != null)
                {
                    Db.Kanoter.Attach(reg.Kanot);
                }

                if (reg.Klass != null)
                {
                    Db.Klasser.Attach(reg.Klass);
                }

                var ev = Db.Evenemang.Include("Registreringar").Where(e => e.Id == reg.EvenemangsId).FirstOrDefault();
                ev.Registreringar.Add(reg);

                Db.SaveChanges();
            }
            catch (Exception exc)
            {
                log.Error("Error while saving a new registration.", exc);
                throw new Exception("Error while saving a new registration", exc);
            }
        }

        /// <summary>
        /// Spara en ny registrering i databasen
        /// </summary>
        /// <param name="wizard"></param>
        public Registreringar SparaNyRegistrering(WizardViewModel wizard)
        {
            try
            {
                log.Debug("Sparar ny registrering");
                var reg = Helpers.ClassMapper.MapToRegistreringar(wizard);
                log.Debug("Konverterat wizard till registrering för lag " + reg.Lagnamn);

                SparaNyRegistrering(reg);

                log.Debug("Sparat registrering för lag " + reg.Lagnamn);

                Db.Entry(reg).GetDatabaseValues();
                return reg;
            }
            catch (Exception exc)
            {
                log.Error("Error while saving new registration.", exc);
                throw new Exception("Error while saving a new registration", exc);
            }
        }

        public void UpdateraRegistrering(Registreringar updatedReg)
        {
            try
            {
                var origReg = Db.Registreringar.Find(updatedReg.Id);
                Db.Entry(updatedReg).CurrentValues.SetValues(origReg);
                Db.SaveChanges();
            }
            catch (Exception exc)
            {
                log.Error("Error updating registration", exc);
                throw new Exception("Error while updating a registration", exc);
            }
        }

        public void TabortRegistrering(Registreringar reg)
        {
            try
            {
                Db.Registreringar.Remove(reg);
                Db.SaveChanges();
                log.Debug("Removed registration with id: " + reg.Id + " and Lagnamn: " + reg.Lagnamn);
            }
            catch (Exception exc)
            {
                log.Error("Error while deleting a registration", exc);
                throw new Exception("Error while deleting a registration", exc);
            }
        }

        public void HarBetalt(Registreringar reg)
        {
            reg.HarBetalt = true;
            UpdateraRegistrering(reg);
        }

        public Evenemang HamtaEvenemang(int id)
        {
            if (CurrentEvenemang == null || CurrentEvenemang.Id != id)
            {
                CurrentEvenemang = Db.Evenemang.Find(id);
            }
            return CurrentEvenemang;
        }

        /// <summary>
        /// Disposing
        /// </summary>
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        /// <summary>
        /// Returnerar den första förseningsavgiften för angivet evenemang och som
        /// ligger inom tidsspannet
        /// </summary>
        /// <param name="evenemangsId"></param>
        /// <returns></returns>
        public Forseningsavgift HamtaForseningsavfigt(int evenemangsId)
        {
            var forseningsavgifterQuery = from avgift in Db.Forseningsavgift
                                   where avgift.EvenemangsId == evenemangsId
                                   && avgift.FranDatum <= DateTime.Now
                                   && avgift.TillDatum > DateTime.Now
                                   select avgift;
            return forseningsavgifterQuery.FirstOrDefault();
        }
    }
}