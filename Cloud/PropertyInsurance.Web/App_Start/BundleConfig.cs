﻿using System.Web;
using System.Web.Optimization;

namespace PropertyInsurance.Web
{
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

            bundles.Add(new ScriptBundle("~/bundles/claim").Include("~/Scripts/claim.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/login.css"));

            bundles.Add(new StyleBundle("~/Content/detail").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/detail.css"));

            bundles.Add(new StyleBundle("~/Content/builders").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/Builders.css"));
            bundles.Add(new StyleBundle("~/Content/claims").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css",
                "~/Content/Builders.css",
                "~/Content/Claims.css"));
        }
    }
}
