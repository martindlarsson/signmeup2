using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignMeUp2.Controllers
{
    public class ErrorController : BaseController
    {
        public ViewResult Index()
        {
            return View("Error");
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }

        protected override string GetEntitetsNamn()
        {
            return string.Empty;
        }
    }
}