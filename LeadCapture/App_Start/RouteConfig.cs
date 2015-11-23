using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IDC.LeadCapture
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");

            routes.MapRoute(
                "ResponseKey",
                "{id}",
                new { controller = "Home", action = "Index" },
                new { id = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b" }
            );

            // example: ~/EmailCsv/C3881723-5F90-4FE6-AA5D-F69D3D01D41B/5/2
            routes.MapRoute(
                "EmailCsv",
                "EmailCsv/{id}/{numberofdays}/{dayofweek}",
                new
                {
                    controller = "Report",
                    action = "EmailCsv",
                    id = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b",
                    numberofdays = UrlParameter.Optional,
                    dayofweek = UrlParameter.Optional
                }
            );

            // example: ~/ChangePassword/C3881723-5F90-4FE6-AA5D-F69D3D01D41B/admin@idc.com
            routes.MapRoute(
                "ChangePassword",
                "Account/ChangePassword/{key}/{username}",
                new
                {
                    controller = "Account",
                    action = "ChangePassword",
                    key = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b", 
                    username = ""
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}