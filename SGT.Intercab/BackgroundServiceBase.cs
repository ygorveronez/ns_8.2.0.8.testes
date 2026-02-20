using Dominio.Excecoes.Embarcador;
using Utilidades.Extensions;

namespace SGT.Intercab
{
    public class BackgroundServiceBase : BackgroundService
    {
        public readonly ILogger<WindowsBackgroundService> _logger;
        public readonly IConfiguration _configuration;

        public BackgroundServiceBase(ILogger<WindowsBackgroundService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? _tipoServicoMultisoftware;
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
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

        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
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

        private string? _host;
        public string? Host
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_host))
                    _host = _configuration.GetValue<string>("Host");

                return _host;
            }
        }

        private string? _stringConexao;
        public string? StringConexao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexao))
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
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

        private string? _stringConexaoAdmin;
        public string? StringConexaoAdmin
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

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";";
        }

        public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        {
            try
            {
                string? filePath = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppContext.BaseDirectory, "appsettings.json");
                string json = Utilidades.IO.FileStorageService.LocalStorage.ReadAllText(filePath);

                dynamic? jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                SetValueRecursively(sectionPathKey, jsonObj, value);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                Utilidades.IO.FileStorageService.LocalStorage.WriteAllText(filePath, output);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            string[]? remainingSections = sectionPathKey.Split(":", 2);

            string? currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                string? nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = value;
            }
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
