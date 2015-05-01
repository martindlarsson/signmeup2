using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignMeUp2.DataModel;

namespace SignMeUp2.Controllers
{
    public class HomeController : Controller
    {
        private SignMeUpDataModel db = new SignMeUpDataModel();

        // GET: Home
        public ActionResult Index()
        {
            return View(db.Evenemang.ToList());
        }
    }
}