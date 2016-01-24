using System.Web.Mvc;
using SignMeUp2.Controllers;

namespace SignMeUp2.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private static string _entity = "Admin";
        protected override string GetEntitetsNamn()
        {
            return _entity;
        }

        // GET: Admin/Admin
        public ActionResult Index()
        {
            var ev = HamtaEvenemangForAnv();

            //ViewBag.Evenemang = new SelectList(ev, "Id", "Namn");
            // Lägg till ngt i ViewBag som fyller dropdown för val av evenemang

            // if role admin then show all
            ViewBag.IsAdmin = User.IsInRole("admin");

            ViewBag.Org = HamtaOrganisation();

            return View(ev);
        }
    }
}
