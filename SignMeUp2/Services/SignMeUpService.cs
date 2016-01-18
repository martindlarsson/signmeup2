using System;
using System.Linq;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
using log4net;
using System.Data.Entity;
using SignMeUp2.Helpers;

namespace SignMeUp2.Services
{
    public class SignMeUpService : IDisposable
    {
        protected readonly ILog log;
        private SignMeUpDataModel _context;

        private Evenemang CurrentEvenemang { get; set; }

        public SignMeUpService()
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
                else if (_context.IsDisposed)
                {
                    log.Warn("DbContext var disposed, skapar en ny instans.");
                    _context = new SignMeUpDataModel();
                }
                return _context;
            }
        }

        internal TabellViewModel GetLista(int value)
        {
            var listan = Db.Listor.Include(l => l.Falt).Include(l => l.Formular).Include(l => l.Formular.Registreringar).Single(l => l.Id == value);
            if (listan == null)
                return null;

            //var formular = Db.Formular.Single(f => f.Id == listan.FormularId);
            
            // TODO mappa till en view model
            return ClassMapper.MappaTillTabell(listan);
        }

        public FormularViewModel GetFormular(int formularsId)
        {
            var formular = Db.Formular.Include(r => r.Evenemang).Single(f => f.Id == formularsId);
            return ClassMapper.MappaTillFormular(formular);
        }

        public Registrering GetRegistrering(int id, bool fill)
        {
            if (fill)
            {
                return Db.Registreringar
                        .Include(r => r.Formular)
                        .Include(r => r.Formular.Evenemang)
                        .Include(r => r.Formular.Evenemang.Organisation)
                        .Include(r => r.Invoice)
                        .Where(r => r.Id == id)
                        .FirstOrDefault();
            }

            return Db.Registreringar.Find(id);
        }

        /// <summary>
        /// Spara ny registrering i databasen
        /// </summary>
        /// <param name="reg"></param>
        public void SparaNyRegistrering(Registrering reg)
        {
            log.Debug("Sparar ny registrering");

            try
            {
                reg.Registreringstid = DateTime.Now;

                Db.Registreringar.Add(reg);

                Db.SaveChanges();
            }
            catch (Exception exc)
            {
                log.Error("Error while saving a new registration.", exc);
                throw new Exception("Error while saving a new registration", exc);
            }
        }

        internal Registrering Spara(SignMeUpVM SUPVM)
        {
            log.Debug("Sparar registrering");

            try
            {
                var reg = ClassMapper.MappaTillRegistrering(SUPVM, this);
                Db.Registreringar.Add(reg);
                Db.SaveChanges();
                Db.Entry(reg).GetDatabaseValues();
                return reg;
            }
            catch (Exception exc)
            {
                log.Error("Ett fel inträffade vid sparande av en registrering.", exc);
                throw new Exception("Ett fel inträffade vid sparande av en registrering.", exc);
            }
        }

        public void UpdateraRegistrering(Registrering updatedReg)
        {
            try
            {
                log.Debug("Uppdaterar registrering. Lagnamn: ?"); // + updatedReg.Lagnamn);
                var origReg = GetRegistrering(updatedReg.Id, false);
                Db.Entry(updatedReg).CurrentValues.SetValues(origReg);
                Db.SaveChanges();
            }
            catch (Exception exc)
            {
                log.Error("Error updating registration", exc);
                throw new Exception("Error while updating a registration", exc);
            }
        }

        public void TabortRegistrering(Registrering reg)
        {
            try
            {
                Db.Registreringar.Remove(reg);
                Db.SaveChanges();

                log.Debug("Removed registration with id: " + reg.Id); // + " and Lagnamn: " + reg.Lagnamn);
            }
            catch (Exception exc)
            {
                log.Error("Error while deleting a registration", exc);
                throw new Exception("Error while deleting a registration", exc);
            }
        }

        public void HarBetalt(Registrering reg)
        {
            var registrering = GetRegistrering(reg.Id, false);
            registrering.HarBetalt = true;
            UpdateraRegistrering(registrering);
        }

        public Evenemang HamtaEvenemang(int id)
        {
            if (CurrentEvenemang == null || CurrentEvenemang.Id != id)
            {
                CurrentEvenemang = Db.Evenemang.Include("Organisation").Include("Organisation.Betalningsmetoder").FirstOrDefault(e => e.Id == id);
            }
            return CurrentEvenemang;
        }

        /// <summary>
        /// Returnerar den första förseningsavgiften för angivet evenemang och som
        /// ligger inom tidsspannet
        /// </summary>
        /// <param name="evenemangsId"></param>
        /// <returns></returns>
        public ForseningsavgiftVM HamtaForseningsavfigt(int evenemangsId)
        {
            var forseningsavgifterQuery = from avgift in Db.Forseningsavgift
                                   where avgift.EvenemangsId == evenemangsId
                                   && avgift.FranDatum <= DateTime.Now
                                   && avgift.TillDatum > DateTime.Now
                                   select avgift;
            var f = forseningsavgifterQuery.FirstOrDefault();

            if (f == null)
            {
                return null;
            }

            return new ForseningsavgiftVM
            {
                FranDatum = f.FranDatum,
                Id = f.Id,
                Namn = f.Namn,
                PlusEllerMinus = f.PlusEllerMinus,
                Summa = f.Summa,
                TillDatum = f.TillDatum
            };
        }

        public Organisation HamtaOrganisation(int formularsId)
        {
            var formular = Db.Formular.Include(f => f.Evenemang).Single(f => f.Id == formularsId);

            return formular != null ? Db.Organisationer.Include("Betalningsmetoder").Single(o => o.Id == formular.Evenemang.OrganisationsId) : null;
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