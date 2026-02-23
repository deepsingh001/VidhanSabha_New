using GlobalVidhanSabha.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GlobalVidhanSabha
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            // Register Exception Filter
            config.Filters.Add(new GlobalExceptionHandler.ExceptionFilter());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
