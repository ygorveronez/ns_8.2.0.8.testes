using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace ReportApi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Servicos.Log.TratarErro("ReportAPI Iniciada.", "StartAPI");

#if !DEBUG
            CriptografarWebConfig("connectionStrings");
#endif

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            CultureConfig.RegisterCulture();
            var provider = DependencyInjection.Configure();

            ControllerBuilder.Current.SetControllerFactory(new MsDiControllerFactory(provider));

            ConnectionFactory.ConfigureFileStorage();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Servicos.Log.TratarErro("ReportAPI Encerrada.", "StartAPI");
        }

        private void CriptografarWebConfig(string section, string applicationPath = null, string provider = "RSAProtectedConfigurationProvider")
        {
            if (applicationPath == null)
                applicationPath = "~";

            Configuration confg = WebConfigurationManager.OpenWebConfiguration(applicationPath);
            ConfigurationSection confStrSect = confg.GetSection(section);

            if (confStrSect != null && !confStrSect.SectionInformation.IsProtected)
            {
                confStrSect.SectionInformation.ProtectSection(provider);
                confg.Save();
            }
        }
    }

    public class MsDiControllerFactory : DefaultControllerFactory
    {
        private readonly IServiceProvider provider;

        public MsDiControllerFactory(IServiceProvider provider) => this.provider = provider;

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            try
            {
                IServiceScope scope = this.provider.CreateScope();

                HttpContext.Current.Items[typeof(IServiceScope)] = scope;

                return (IController)scope.ServiceProvider.GetRequiredService(controllerType);
            }
            catch (Exception ex)
            {
                //Servicos.Log.TratarErro(ex);
                //Enterar a exception e vida que segue;
                return null;
            }

        }

        public override void ReleaseController(IController controller)
        {
            base.ReleaseController(controller);

            var scope = HttpContext.Current.Items[typeof(IServiceScope)] as IServiceScope;

            scope?.Dispose();
        }
    }
}