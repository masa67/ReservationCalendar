using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace ReservationCalendar
{
    internal class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    internal static class BundleExtensions
    {
        public static Bundle ForceOrdered(this Bundle sb)
        {
            sb.Orderer = new AsIsBundleOrderer();
            return sb;
        }
    }

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-route.js",
                      "~/Scripts/angular-resource.js"));

            bundles.Add(new StyleBundle("~/bundles/metronic/css").ForceOrdered()
                // BEGIN GLOBAL MANDATORY STYLES
                .Include("~/Metronic/global/plugins/font-awesome/css/font-awesome.css")
                .Include("~/Metronic/global/plugins/simple-line-icons/simple-line-icons.css")
                .Include("~/Metronic/global/plugins/bootstrap/css/bootstrap.css")
                .Include("~/Metronic/global/plugins/uniform/css/uniform.default.css")
                .Include("~/Metronic/global/plugins/bootstrap-switch/css/bootstrap-switch.css")
                // END GLOBAL MANDATORY STYLES
                // BEGIN PAGE LEVEL PLUGIN STYLES
                .Include("~/Metronic/global/plugins/gritter/css/jquery.gritter.css")
                .Include("~/Metronic/global/plugins/bootstrap-daterangepicker/daterangepicker-bs3.css")
                .Include("~/Metronic/global/plugins/fullcalendar/fullcalendar/fullcalendar.css")
                .Include("~/Metronic/global/plugins/jqvmap/jqvmap/jqvmap.css")
                // END PAGE LEVEL PLUGIN STYLES
                // BEGIN PAGE STYLES
                .Include("~/Metronic/admin/pages/css/tasks.css")
                // END PAGE STYLES
                // BEGIN THEME STYLES
                .Include("~/Metronic/global/css/components.css")
                .Include("~/Metronic/global/css/plugins.css")
                .Include("~/Metronic/admin/layout/css/layout.css")
                .Include("~/Metronic/admin/layout/css/themes/default.css")
                .Include("~/Metronic/admin/layout/css/custom.css"));
                // END THEME STYLES

            bundles.Add(new ScriptBundle("~/bundles/metronic").Include());
                      
            bundles.Add(new ScriptBundle("~/bundles/calendarApp").Include(
                      "~/Areas/Calendar/Scripts/client/app.js",
                      "~/Areas/Calendar/Scripts/client/Controllers/calendarCtrl.js"));
        }
    }
}
