using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using SignMeUp2.Data;

namespace SignMeUp2.Helpers
{
    public class EvenemangHelper
    {
        public enum EvenemangValidationResult
        {
            DoesNotExist = 0,
            NotOpen = 1,
            Closed = 2,
            OK = 3
        }

        public static EvenemangValidationResult EvaluateEvenemang(Evenemang ev)
        {
            if (ev != null) {
                var start = ev.RegStart;
                if (start != null && DateTime.Now < start)
                {
                    return EvenemangValidationResult.NotOpen;
                }

                var slut = ev.RegStop;
                if (slut != null && DateTime.Now >= slut)
                {
                    return EvenemangValidationResult.Closed;
                }
            }
            else
                return EvenemangValidationResult.DoesNotExist;

            return EvenemangValidationResult.OK;
        }

        public static void UpdateLanguage(Språk språk)
        {
            if (språk == Språk.Engelska)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
        }
    }
}