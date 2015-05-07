using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SignMeUp2.Models;
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
            //var configuration = new SignMeUp2.Migrations.Configuration();
            //var migrator = new DbMigrator(configuration);
            //migrator.Update();
            System.Data.Entity.Database.SetInitializer<DataModel.SignMeUpDataModel>(new System.Data.Entity.MigrateDatabaseToLatestVersion<DataModel.SignMeUpDataModel, Migrations.Configuration>());
        }
    }
}
