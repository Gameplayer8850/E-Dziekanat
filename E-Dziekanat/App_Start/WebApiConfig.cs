using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace E_Dziekanat
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Access-Control-Allow-Origin
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Konfiguracja i usługi składnika Web API
            // Skonfiguruj składnik Web API, aby korzystał tylko z uwierzytelniania za pomocą tokenów bearer.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //Dokumenty dodanie formatowania - Bartek
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));

            // Trasy składnika Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
