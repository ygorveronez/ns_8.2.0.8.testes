using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmissaoCTe.InternalWebAdmin
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#else
            CriptografarWebConfig("connectionStrings");
#endif
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.AppRelativeCurrentExecutionFilePath == "~/")
                HttpContext.Current.RewritePath("Default.aspx");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            if (Session["IdUsuario"] == null)
                return;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.LogAcesso repLogAcesso = new Repositorio.LogAcesso(unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo((int)Session["IdUsuario"]);
                Dominio.Entidades.LogAcesso logAcesso = new Dominio.Entidades.LogAcesso();

                logAcesso.Data = DateTime.Now;
                logAcesso.IPAcesso = string.Empty; //Request não fica disponível neste contexto, por isso não é salvo. Para identificar qual a sessão de início deve-se verificar pelo SessionID.
                logAcesso.Login = usuario.Login;
                logAcesso.Senha = usuario.Senha;
                logAcesso.SessionID = Session.SessionID;
                logAcesso.Tipo = Dominio.Enumeradores.TipoLogAcesso.Saída;
                logAcesso.Usuario = usuario;

                repLogAcesso.Inserir(logAcesso);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Start", "Default.aspx", new { controller = "Home", action = "Index", id = "" });
            routes.MapRoute("Api", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
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
}