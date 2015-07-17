using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;
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
        /// <param name="SUPVM"></param>
        public Registreringar SparaNyRegistrering(SignMeUpVM SUPVM)
        {
            try
            {
                log.Debug("Sparar ny registrering");
                //var reg = Helpers.ClassMapper.MapToRegistreringar(SUPVM);
                //log.Debug("Konverterat wizard till registrering för lag " + reg.Lagnamn);

                //SparaNyRegistrering(reg);

                //log.Debug("Sparat registrering för lag " + reg.Lagnamn);

                //Db.Entry(reg).GetDatabaseValues();
                return null;
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
                log.Debug("Uppdaterar registrering. Lagnamn: " + updatedReg.Lagnamn);
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

        public Organisation HamtaOrganisation(int organisationsId)
        {
            return Db.Organisationer.Include("Betalningsmetoder").Single(o => o.Id == organisationsId);
        }

        public SelectList HamtaBanor(int evenemangsId)
        {
            return new SelectList(Db.Banor.Where(b => b.EvenemangsId == evenemangsId).ToList(), "ID", "Namn");
        }

        public SelectList HamtaKanter(int evenemangsId)
        {
            return new SelectList(Db.Kanoter.Where(b => b.EvenemangsId == evenemangsId).ToList(), "ID", "Namn");
        }

        public SelectList HamtaKlasser(int evenemangsId)
        {
            return new SelectList(Db.Klasser.Where(b => b.EvenemangsId == evenemangsId).ToList(), "ID", "Namn");
        }

        public IList<WizardStep> HamtaWizardSteps(int evenemangsId)
        {
            var list = new List<WizardStep> {
                // Registrering
                new WizardStep
                {
                    Namn = "Registrering",
                    StepIndex = 0,
                    StepCount = 3,
                    FaltLista = new List<FaltViewModel>
                    {
                        new FaltViewModel {
                            Namn = "Lagnamn",
                            Kravs = true,
                            Typ = FaltTyp.text_falt
                        },
                        new FaltViewModel {
                            Namn = "Bana",
                            Kravs = true,
                            Alternativ = HamtaBanor(evenemangsId),
                            Typ = FaltTyp.val_falt
                        },
                        new FaltViewModel {
                            Namn = "Klass",
                            Kravs = true,
                            Alternativ = HamtaKlasser(evenemangsId),
                            Typ = FaltTyp.val_falt
                        },
                        new FaltViewModel {
                            Namn = "Kanot",
                            Kravs = true,
                            Alternativ = HamtaKanter(evenemangsId),
                            Typ = FaltTyp.val_falt
                        }
                    }
                },
                // Deltagare
                new WizardStep
                {
                    Namn = "Deltagare",
                    StepIndex = 1,
                    StepCount = 3
                },
                // Kontaktinformation
                new WizardStep
                {
                    Namn = "Kontaktinformation",
                    StepIndex = 2,
                    StepCount = 3,
                    FaltLista = new List<FaltViewModel>
                    {
                        new FaltViewModel {
                            Namn = "Adress",
                            Kravs = true,
                            Typ = FaltTyp.text_falt
                        },
                        new FaltViewModel {
                            Namn = "Telefon",
                            Kravs = true,
                            Typ = FaltTyp.text_falt
                        },
                        new FaltViewModel {
                            Namn = "Epost",
                            Kravs = true,
                            Typ = FaltTyp.text_falt
                        },
                        new FaltViewModel {
                            Namn = "Klubb",
                            Kravs = false,
                            Typ = FaltTyp.text_falt
                        }
                    }
                },
            };

            return list;
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