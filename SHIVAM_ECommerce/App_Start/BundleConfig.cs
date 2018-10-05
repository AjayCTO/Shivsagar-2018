using System.Web;
using System.Web.Optimization;

namespace SHIVAM_ECommerce
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/JqueryJS").Include(
                        "~/Scripts/jquery-1.12.4.js",
                        "~/Scripts/jquery-ui.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/dataTableScripts").Include(
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.bootstrap4.min.js",
                        "~/Scripts/dataTables.rowGroup.min.js",
                        "~/Scripts/dataTables.responsive.min.js"

                        ));

            bundles.Add(new StyleBundle("~/Content/dataTableStyles").Include(
                        "~/Content/dataTables.bootstrap4.min.css",
                        "~/Content/responsive.bootstrap4.min.css",
                        "~/Content/buttons.dataTables.min.css",
                        "~/Content/rowGroup.dataTables.min.css",
                        "~/Content/responsive.dataTables.min.css"
                  ));


            bundles.Add(new ScriptBundle("~/bundles/TabularThemeAssetsJS").Include(
                      "~/Content/TabularTheme/assets/js/require.min.js",
                      "~/Content/TabularTheme/assets/js/dashboard.js",
                      "~/Content/TabularTheme/assets/plugins/charts-c3/plugin.js",
                      "~/Content/TabularTheme/assets/plugins/maps-google/plugin.js",
                      "~/Content/TabularTheme/assets/plugins/input-mask/plugin.js"
                //"~/Scripts/toastr.js"
                       ));


            bundles.Add(new StyleBundle("~/Content/TabularThemeAssetsCSS").Include(
                      "~/Content/TabularTheme/assets/css/dashboard.css",
                      "~/Content/TabularTheme/assets/plugins/charts-c3/plugin.css",
                      "~/Content/TabularTheme/assets/plugins/maps-google/plugin.css",
                      "~/Content/toaster.min.css"
                  ));


            bundles.Add(new StyleBundle("~/Content/JqueryUiCSS").Include(
                      "~/Content/jquery-ui.css"
                  ));


            bundles.Add(new StyleBundle("~/Content/RandomCSS").Include(
                      "~/Content/font-awesome.min.css",
                      "~/Content/css.css"
                  ));







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
                        "~/Scripts/bootbox.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                        "~/Content/dataTables.bootstrap.min.css"

                  ));
        }
    }
}
