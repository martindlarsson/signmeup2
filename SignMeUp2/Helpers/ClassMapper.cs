﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.DataModel;
using SignMeUp2.Models;

namespace SignMeUp2.Helpers
{
    public class ClassMapper
    {
        public static Registreringar MapToRegistreringar(WizardViewModel wizard)
        {
            var reg = new Registreringar();

            reg.Evenemang_Id = wizard.Evenemang_Id;

            reg.Invoices = wizard.Fakturaadress;

            foreach (IWizardStep step in wizard.Steps)
            {
                if (step is RegistrationViewModel)
                {
                    var regStep = (RegistrationViewModel)step;
                    reg.Lagnamn = regStep.Lagnamn;
                    reg.Bana = regStep.Bana;
                    reg.Klass = regStep.Klass;
                    reg.Kanot = regStep.Kanot;
                    reg.Ranking = regStep.Ranking;
                }
                else if (step is ContactViewModel)
                {
                    var contStep = (ContactViewModel)step;
                    reg.Adress = contStep.Adress;
                    reg.Epost = contStep.Epost;
                    reg.Telefon = contStep.Telefon;
                    reg.Klubb = contStep.Klubb;
                }
                else if (step is DeltagareListViewModel)
                {
                    reg.Deltagare = new List<Deltagare>();
                    var deltagarlistaStep = (DeltagareListViewModel)step;
                    foreach (var deltagareNew in deltagarlistaStep.DeltagareLista)
                    {
                        var deltagare = new Deltagare();
                        deltagare.Förnamn = deltagareNew.Förnamn;
                        deltagare.Efternamn = deltagareNew.Efternamn;
                        deltagare.Personnummer = deltagareNew.Personnummer;
                        reg.Deltagare.Add(deltagare);
                    }
                }
            }

            return reg;
        }
    }
}