using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.ServiceProcess;

namespace SGT.Monitoramento
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

            try
            {

                Utilidades.File.CreateAppSettingsJsonIfNotExist(AppDomain.CurrentDomain.BaseDirectory);

                var builder = new ConfigurationBuilder()
                                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                  .AddJsonFile("appsettings.json");

                appSettings = builder.Build();
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                throw;
            }

#if !DEBUG
            ServiceBase[] ServicesToRun = new ServiceBase[]
                {
                    new ServicoMonitoramento()
                };

                ServiceBase.Run(ServicesToRun);
#else

            ServicoMonitoramento svc = new ServicoMonitoramento();
            svc.SetarExecucaoThreads();

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#endif
        }

        private static IConfigurationRoot appSettings;

        private static AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? _tipoServicoMultisoftware;
        public static AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
        {
            get
            {
                if (!_tipoServicoMultisoftware.HasValue)
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin))
                    {
                        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Host);

                        if (clienteURLAcesso == null)
                            throw new Exception($"O Host {Host} não possui uma configuração");

                        _tipoServicoMultisoftware = clienteURLAcesso.TipoServicoMultisoftware;
                    }
                }

                return _tipoServicoMultisoftware.Value;
            }
        }

        public static AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
        {
            get
            {
                using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin))
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Host);

                    if (clienteURLAcesso == null)
                        throw new Exception($"O Host {Host} não possui uma configuração");

                    return clienteURLAcesso.Cliente;
                }
            }
        }

        private static string _host;
        public static string Host
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_host))
                    _host = appSettings["AppSettings:Host"];
                return _host;
            }
        }

        private static string _stringConexao;
        public static string StringConexao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexao))
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin))
                    {
                        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Host);

                        if (clienteURLAcesso == null)
                            throw new Exception($"O Host {Host} não possui uma configuração");

                        if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                            _stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                        else
                            _stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);
                    }
                }

                return _stringConexao;
            }
        }

        private static string _stringConexaoAdmin;
        public static string StringConexaoAdmin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexaoAdmin))
                    _stringConexaoAdmin = System.Configuration.ConfigurationManager.ConnectionStrings["AdminMultisoftware"].ConnectionString;

                return _stringConexaoAdmin;
            }
        }

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";";
        }

    }
}
