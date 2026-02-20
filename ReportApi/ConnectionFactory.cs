using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilidades.Extensions;

namespace ReportApi
{
    public static class ConnectionFactory
    {        
        private static string _host;

        public static string Host
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_host))
                {
                    _host = System.Configuration.ConfigurationManager.AppSettings["Host"];
#if DEBUG
                    _host = ObterArquivoURLDebug().FirstOrDefault();
#endif
                }

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

                        if (clienteURLAcesso.URLHomologacao &&
                            (clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null || clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacaoReport != null))
                            _stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacaoReport ?? clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                        else
                            _stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoReport ?? clienteURLAcesso.Cliente.ClienteConfiguracao);

#if DEBUG
                        _stringConexao = ObterConnectionStringDebug(_stringConexao);
#endif
                        Servicos.Log.SetStringConexao(_stringConexao);
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
                {
#if DEBUG
                    _stringConexaoAdmin = ObterConnectionAdminDebug();
#else
                    _stringConexaoAdmin = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
#endif
                }

                return _stringConexaoAdmin;
            }
        }

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

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Application Name=ReportApi;Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";Max Pool Size=600;";
        }

        private static AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;
        public static AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
        {
            get
            {
                if (_cliente == null)
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin))
                    {
                        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Host);

                        if (clienteURLAcesso == null)
                            throw new Exception($"O Host {Host} não possui uma configuração");

                        _cliente = clienteURLAcesso.Cliente;
                    }
                }

                return _cliente;
            }
        }

        public static void ConfigureFileStorage()
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);
        }

        #region DebugOptions

        private static List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new ControllerException("Arquivo DebugConfig.txt não localizado.");
        }

        private static string ObterConnectionAdminDebug()
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                throw new ControllerException("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

            dynamic configDebug = json.FromJson<dynamic>();

            string connectionString = (string)configDebug.AdminMultisoftware;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ControllerException("String de conexão do AdminMultisoftware não encontrada.");

            return connectionString;
        }

        private static string ObterConnectionStringDebug(string connectionDefault)
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return connectionDefault;

            dynamic configDebug = json.FromJson<dynamic>();

            return !string.IsNullOrWhiteSpace((string)configDebug.ConnectionString) ? (string)configDebug.ConnectionString : connectionDefault;
        }

        #endregion

    }
}
