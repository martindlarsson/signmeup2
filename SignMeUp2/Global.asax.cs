using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SignMeUp2.ViewModels;
using SignMeUp2.Data;
using SignMeUp2.Services;
using log4net;
using System.Data.Entity.Migrations;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SignMeUp2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected readonly ILog log;

        public MvcApplication()
        {
            log = LogManager.GetLogger(GetType());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //ModelBinders.Binders.Add(typeof(WizardStep), new WizardModelBinder());

            // Run migrations at startup
            System.Data.Entity.Database.SetInitializer<SignMeUpDataModel>(new System.Data.Entity.MigrateDatabaseToLatestVersion<SignMeUpDataModel, Migrations.Configuration>());

            log.Info("Application startad.");
        }

        void Session_Start(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Add("__MyAppSession", string.Empty);
        }

        //protected virtual void Application_BeginRequest()
        //{
        //    //log.Info("Application_BeginRequest");
        //}

        //protected virtual void Application_EndRequest()
        //{
        //    var service = SignMeUpService.Instance;
        //    service.Dispose();
        //}
    }
}
