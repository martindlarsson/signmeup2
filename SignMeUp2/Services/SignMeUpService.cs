using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using SignMeUp2.Data;
using SignMeUp2.Models;
using log4net;
using System.Data.Entity;

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
                if (_context == null || _context.IsDisposed)
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
            var reg = Db.Registreringar
                .Include("Bana")
                .Include("Klass")
                .Include("Kanot")
                .Include("Deltagare")
                .Where(r => r.Id == id)
                .FirstOrDefault();

            return fill && reg != null ? FillRegistrering(reg) : reg;
        }

        public Registreringar FillRegistrering(Registreringar reg)
        {
            reg.Evenemang = Db.Evenemang.Find(reg.EvenemangsId);
            reg.Evenemang.Organisation = Db.Organisationer.Find(reg.Evenemang.OrganisationsId);

            if (reg.Rabatt == null && reg.RabattId != 0)
            {
                reg.Rabatt = Db.Rabatter.Find(reg.RabattId);
            }

            if (reg.Forseningsavgift == null && reg.ForseningsavgiftId != 0)
            {
                reg.Forseningsavgift = Db.Forseningsavgift.Find(reg.ForseningsavgiftId);
            }

            return reg;
        }

        /// <summary>
        /// Spara ny registrering i databasen
        /// </summary>
        /// <param name="reg"></param>
        public void SparaNyRegistrering(Registreringar reg)
        {
            log.Debug("Sparar ny registrering");

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

                if (reg.Evenemang != null)
                {
                    Db.Evenemang.Attach(reg.Evenemang);
                }

                if (reg.Forseningsavgift != null && reg.ForseningsavgiftId == null)
                {
                    reg.ForseningsavgiftId = reg.Forseningsavgift.Id;
                    reg.Forseningsavgift = null;
                }

                if (reg.Rabatt != null && reg.RabattId == null)
                {
                    reg.RabattId = reg.Rabatt.Id;
                    reg.Rabatt = null;
                }

                var ev = Db.Evenemang.Include("Registreringar").Where(e => e.Id == reg.EvenemangsId).FirstOrDefault();
                ev.Registreringar.Add(reg);

                Db.SaveChanges();
            }
            catch (Exception exc)
            {
                //Trace.TraceError("Fel vid sparande av registrering. Exc: " + exc.Message + " ST: " + exc.StackTrace);
                log.Error("Error while saving a new registration.", exc);
                throw new Exception("Error while saving a new registration", exc);
            }
        }

        /// <summary>
        /// Spara en ny registrering i form av en wizard i databasen
        /// </summary>
        /// <param name="wizard"></param>
        public Registreringar SparaNyRegistrering(WizardViewModel wizard)
        {
            try
            {
                //Trace.TraceInformation("Sparar ny registrering");
                log.Debug("Sparar ny registrering");
                var reg = Helpers.ClassMapper.MapToRegistreringar(wizard);
                log.Debug("Konverterat wizard till registrering för lag " + reg.Lagnamn);

                SparaNyRegistrering(reg);

                Trace.TraceInformation("Sparat registrering för lag " + reg.Lagnamn);
                log.Debug("Sparat registrering för lag " + reg.Lagnamn);

                Db.Entry(reg).GetDatabaseValues();
                return reg;
            }
            catch (Exception exc)
            {
                //Trace.TraceError("Fel vid sparande av registrering. Exc: " + exc.Message + " ST: " + exc.StackTrace);
                log.Error("Error while saving new registration.", exc);
                throw new Exception("Error while saving a new registration", exc);
            }
        }

        public void UpdateraRegistrering(Registreringar updatedReg)
        {
            try
            {
                Trace.TraceInformation("Uppdaterar registrering");
                var origReg = GetRegistrering(updatedReg.Id, false);
                Db.Entry(updatedReg).CurrentValues.SetValues(origReg);
                Db.SaveChanges();
            }
            catch (Exception exc)
            {
                //Trace.TraceError("Fel vid uppdatering av registrering. Exc: " + exc.Message + " ST: " + exc.StackTrace);
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
                //Trace.TraceInformation("Removed registration with id: " + reg.Id + " and Lagnamn: " + reg.Lagnamn);
                log.Debug("Removed registration with id: " + reg.Id + " and Lagnamn: " + reg.Lagnamn);
            }
            catch (Exception exc)
            {
                //Trace.TraceError("Fel vid borttagning av registrering. Exc: " + exc.Message + " ST: " + exc.StackTrace);
                log.Error("Error while deleting a registration", exc);
                throw new Exception("Error while deleting a registration", exc);
            }
        }

        public void HarBetalt(Registreringar reg)
        {
            var registrering = GetRegistrering(reg.Id, false);
            registrering.HarBetalt = true;
            UpdateraRegistrering(registrering);
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
    }
}