using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SignMeUp2.Models;
using SignMeUp2.DataModel;
using SignMeUp2.Services;
using log4net;
using System.Data.Entity.Migrations;

namespace SignMeUp2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.Add(typeof(IWizardStep), new WizardModelBinder());

            // Run migrations at startup
            System.Data.Entity.Database.SetInitializer<DataModel.SignMeUpDataModel>(new System.Data.Entity.MigrateDatabaseToLatestVersion<DataModel.SignMeUpDataModel, Migrations.Configuration>());
        }

        //protected virtual void Application_BeginRequest()
        //{
        //    var service = SignMeUpService.Instance;
        //}

        protected virtual void Application_EndRequest()
        {
            var service = SignMeUpService.Instance;
            service.Dispose();
            //var entityContext = HttpContext.Current.Items["_EntityContext"] as SignMeUpDataModel;
            //if (entityContext != null)
            //    entityContext.Dispose();
        }
    }
}
