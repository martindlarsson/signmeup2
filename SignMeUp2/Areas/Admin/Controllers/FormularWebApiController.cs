using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using SignMeUp2.Data;

namespace SignMeUp2.Areas.Admin.Controllers
{
    public class FormularWebApiController : ApiController
    {
        private SignMeUpDataModel db = new SignMeUpDataModel();



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
