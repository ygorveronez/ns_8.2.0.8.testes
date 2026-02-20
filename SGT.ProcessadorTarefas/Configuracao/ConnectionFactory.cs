using Dominio.Excecoes.Embarcador;

namespace SGT.ProcessadorTarefas.Configuracao
{
    public static class ConnectionFactory
    {
        #region Propriedades Privadas

        private static string? _stringConexao;
        private static string? _stringConexaoAdmin;

        #endregion Propriedades Privadas

        #region Propriedades Públicas

        public static string StringConexaoAdmin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexaoAdmin))
                {
#if DEBUG
                    _stringConexaoAdmin = ObterConnectionAdminMultisoftwareDebug();
#else
                    _stringConexaoAdmin = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
#endif
                }

                return _stringConexaoAdmin;
            }
        }

        #endregion Propriedades Públicas

        #region Métodos Públicos

        public static string StringConexao(IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(_stringConexao))
            {
                using AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin);

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Host(configuration));

                if (clienteURLAcesso == null)
                {
                    unitOfWork.Dispose();
                    throw new Exception($"O Host {Host(configuration)} não possui uma configuração");
                }

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
                unitOfWork.Dispose();
            }
            return _stringConexao;
        }

        public static void ConfigureFileStorage(IConfiguration configuration)
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host(configuration));
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";Max Pool Size=600;";
        }

        private static Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado() => Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos;

        private static string? Host(IConfiguration configuration)
        {
#if DEBUG
            return ObterArquivoURLDebug().FirstOrDefault();
#else
            return configuration.GetValue<string>("Host");
#endif
        }

        private static string ObterConnectionAdminMultisoftwareDebug()
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                throw new ControllerException("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            string connectionString = (string)configDebug.AdminMultisoftware;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ControllerException("String de conexão do AdminMultisoftware não encontrada.");

            return connectionString;
        }

        private static List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new ControllerException("Arquivo DebugConfig.txt não localizado.");
        }

        private static string ObterConnectionStringDebug(string connectionDefault)
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return connectionDefault;

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            return !string.IsNullOrWhiteSpace((string)configDebug.ConnectionString) ? (string)configDebug.ConnectionString : connectionDefault;
        }

        #endregion Métodos Privados
    }
}
