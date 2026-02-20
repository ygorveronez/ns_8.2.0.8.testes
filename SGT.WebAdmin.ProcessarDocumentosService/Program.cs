using Microsoft.Extensions.Configuration;
using Servicos.Http;
using System;
using System.ServiceProcess;

namespace SGT.WebAdmin.ProcessarDocumentosService
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                Utilidades.File.CreateAppSettingsJsonIfNotExist(AppDomain.CurrentDomain.BaseDirectory);

                var builder = new ConfigurationBuilder()
                                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                  .AddJsonFile("appsettings.json");

                HttpClientRegistration.RegisterClients();
                appSettings = builder.Build();
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                throw;
            }
#if !DEBUG
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ProcessarThreads()
            };
            ServiceBase.Run(ServicesToRun);
#else

            ProcessarThreads svc = new ProcessarThreads();
            svc.ConfigurarThreads();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

#endif
        }

        public static IConfigurationRoot appSettings;

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

        private static Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        public static Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado
        {
            get
            {
                if (_auditado == null)
                {
                    _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    _auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
                    _auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                    //_auditado.Empresa = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) ? Empresa : null;
                    _auditado.Texto = "";
                }
                return _auditado;
            }
        }

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

                        Servicos.Log.SetStringConexao(_stringConexao, 15);
                        Servicos.Log.SetCliente(clienteURLAcesso.Cliente.NomeFantasia + (clienteURLAcesso.URLHomologacao ? " Homo" : " Produção"));
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
                    _stringConexaoAdmin = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");

                return _stringConexaoAdmin;
            }
        }

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";";
        }
    }
}
