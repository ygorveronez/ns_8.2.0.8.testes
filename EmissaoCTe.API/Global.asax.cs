using EmissaoCTe.API.Controllers;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmissaoCTe.API
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(Conexao.StringConexao);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Repositorio.NHibernateHttpModule.CloseSession(Conexao.StringConexao);
        }
    }
}