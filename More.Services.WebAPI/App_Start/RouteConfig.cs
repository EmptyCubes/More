using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace More.Services.WebAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DomainApiById",
                routeTemplate: "api/LookupTable/id/{id}",
                defaults: new { controller = "LookupTable", action = "GetTablebyId" });

            routes.MapHttpRoute(
                name: "DomainApiByIdByEffectiveDate",
                routeTemplate: "api/LookupTable/id/{id}/{effectiveDate}",
                defaults: new { controller = "LookupTable", action = "GetTablebyIdByEffectiveDate" });

            routes.MapHttpRoute(
                name: "DomainApiByNameByGroup",
                routeTemplate: "api/LookupTable/{name}/{group}",
                defaults: new { controller = "LookupTable", action = "GetTableByNameByGroup" });

            routes.MapHttpRoute(
                name: "DomainApiByNameByGroupByEffectiveDateWithExpression",
                routeTemplate: "api/LookupTable/{name}/{group}/{effectiveDate}/{expression}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}