using System.Web;
using System.Web.Optimization;

namespace SignMeUp2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/SignMeUpValidering.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                      "~/Content/bootstrap.superhero.min.css",
                      "~/Content/bootstrap-datetimepicker.min.css",
                      "~/Content/site.css"));

            // För datetimepicker
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datetimepicker").Include(
                "~/Scripts/moment.min.js",
                "~/Scripts/moment-with-locales.min.js",
                "~/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/gridster").Include(
                "~/Scripts/jquery.gridster.js"));


            bundles.Add(new StyleBundle("~/Content/gridstercss").Include(
                      "~/Content/jquery.gridster.css"));

            // applicationInsights.js
            bundles.Add(new ScriptBundle("~/bundles/ai").Include(
                    "~/Scripts/applicationInsights.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
