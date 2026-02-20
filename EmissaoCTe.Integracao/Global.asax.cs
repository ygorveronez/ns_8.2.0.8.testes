using System;
using System.Configuration;
using System.Web.Configuration;

namespace EmissaoCTe.Integracao
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //this.MigrateDatabase(); Não funciona mais em versões antigas do .NET
            this.LoadConfigurations();
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(Conexao.StringConexao);

#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#else
            CriptografarWebConfig("connectionStrings");
#endif

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Repositorio.NHibernateHttpModule.CloseSession(Conexao.StringConexao);
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        private void MigrateDatabase()
        {
            try
            {
                Migrations.Migrator.MigrateToLatest(WebConfigurationManager.ConnectionStrings["ControleCTe"].ConnectionString);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void LoadConfigurations()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
#if !DEBUG
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);

#else
                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
#endif

            }
            finally
            {
                unitOfWork.Dispose();
            }
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