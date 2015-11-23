using System.Web;
using System.Web.Optimization;

namespace IDC.LeadCapture
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.maskedinput.min.js",
                        "~/Assets/js/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/assets/bootstrap").Include(
                        "~/Assets/plugins/bootstrap/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/assets/bootstrap-table").Include(
                        "~/Assets/plugins/bootstrap-table/bootstrap-table.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/assets/sky-forms").Include(
                        "~/Assets/plugins/sky-forms/js/jquery.form.min.js",
                        "~/Assets/plugins/sky-forms/js/jquery.maskedinput.min.js",
                        "~/Assets/plugins/sky-forms/js/jquery.modal.js"));

            bundles.Add(new ScriptBundle("~/bundles/assets/plugins").Include(
                        "~/Assets/plugins/back-to-top.js",
                        "~/Assets/plugins/smoothScroll.js",
                        "~/Assets/plugins/jquery.parallax.js",
                        "~/Assets/plugins/formvalidation/dist/js/formValidation.min.js",
                        "~/Assets/plugins/formvalidation/dist/js/framework/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom-assessment").Include(
                     "~/Scripts/js/custom-assessment.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom-admin").Include(
                     "~/Scripts/js/custom-admin.js"));
            bundles.Add(new StyleBundle("~/bootstrap-table/css").Include(
                 "~/Assets/plugins/bootstrap-table/bootstrap-table.css",
                "~/Assets/plugins/bootstrap-table/bootstrap-table.min.css"));

            bundles.Add(new StyleBundle("~/assets/css").Include(
                // <!-- CSS Global Compulsory -->
                "~/Assets/plugins/bootstrap/css/bootstrap.min.css",
                "~/Assets/plugins/bootstrap/css/bootstrap.min.css",
                "~/Assets/css/style.css",
                // <!-- CSS Header and Footer -->
                "~/Assets/css/headers/header-default.css",
                "~/Assets/css/footers/footer-v2.css",
                // <!-- CSS Implementing Plugins -->
                "~/Assets/plugins/animate.css",
                "~/Assets/plugins/line-icons/line-icons.css",
                "~/Assets/plugins/font-awesome/css/font-awesome.min.css",
                "~/Assets/plugins/sky-forms-pro/skyforms/css/sky-forms.css",
                "~/Assets/plugins/sky-forms-pro/skyforms/custom/custom-sky-forms.css",
                // <!-- CSS Customization -->
                "~/Assets/css/theme-colors/dark-red.css",
                "~/Assets/css/custom.css",
                "~/Assets/css/custom-dp.css",
                "~/Assets/css/custom-landingpage2.css",
                "~/Assets/css/custom-landingpage-final.css",
                "~/Assets/css/spacing.css",
                // <!-- FormValidation CSS file -->
                "~/Assets/plugins/formvalidation/dist/css/formValidation.min.css"));
        }
    }
}