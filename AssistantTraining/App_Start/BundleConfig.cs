using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace AssistantTraining.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            RegisterStyleBundles(bundles);
            RegisterScriptBundles(bundles);
        }


        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/gridmvc").Include(
                     "~/Content/Gridmvc.css"));
        }

        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/gridmvc").Include(
                      "~/Scripts/ladda-bootstrap/*.min.js",
                      "~/Scripts/URI.js",
                      "~/Scripts/gridmvc.min.js",
                      "~/Scripts/gridmvc-ext.js"));

            bundles.Add(new ScriptBundle("~/bundles/application/Training").Include(
          "~/Scripts/Application/Training.js"));

            bundles.Add(new ScriptBundle("~/bundles/application/Reports").Include(
          "~/Scripts/Application/Reports.js"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
                      "~/Scripts/bootstrap3-typeahead.min.js"));
        }
    }
}