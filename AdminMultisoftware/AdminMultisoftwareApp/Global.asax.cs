using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AdminMultisoftwareApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#endif

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
