using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(ReportApi.Startup))]
namespace ReportApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
        }
    }
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

           
        }
    }
}
