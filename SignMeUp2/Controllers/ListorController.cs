using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.Data;
using SignMeUp2.ViewModels;

namespace SignMeUp2.Controllers
{
    public class ListorController : BaseController
    {

        protected override string GetEntitetsNamn()
        {
            return string.Empty;
        }

        // GET: Listas
        public ActionResult Index(int? id)
        {
            if (id == null)
                return ShowError(log, "Inget evenemang angivit. Klicka på länken nedan och välj ett evenemang.", false);

            TabellViewModel lista = smuService.GetLista(id.Value);
            
            return View(lista);
        }

        public ActionResult Organizer(int? id)
        {
            if (id == null)
                return ShowError(log, "Ingen organisatör angiven.", false);

            ICollection<TabellViewModel> listor = smuService.GetListaOrg(id.Value, true);

            var org = smuService.Db.Organisationer.FirstOrDefault(orgz => orgz.Id == id.Value);
            ViewBag.org = org.Namn;

            return View(listor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
