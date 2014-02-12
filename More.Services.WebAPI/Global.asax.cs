using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GlobalCORS_RC.Handlers;
using More.Services.WebAPI.Models;

namespace More.Services.WebAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //var config = GlobalConfiguration.Configuration;
            //config.Formatters.Insert(0, new JsonpMediaTypeFormatter());
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CorsHandler());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("json", "true", "application/json"));

            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy =
    IncludeErrorDetailPolicy.Always;
        }
    }

}