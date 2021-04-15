using Shared;
using Shared.BazaDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace E_Dziekanat
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //umożliwia korzystanie z kontorlerów z innych bibliotek
            ControllerBuilder.Current.DefaultNamespaces.Add("Kokpit.Controllers");
            //pobiera dane z config.xml i nawiązuje połączenie
            Zarzadzanie.NawiazPolaczenieBaza();
        }
    }
}
