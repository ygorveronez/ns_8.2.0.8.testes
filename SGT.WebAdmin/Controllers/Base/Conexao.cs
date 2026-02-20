using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.Cache;
using SGT.BackgroundWorkers.Utils;
using Exception = System.Exception;

namespace SGT.WebAdmin.Controllers
{
    public class Conexao
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DistributedLock> _logger;

        public Conexao(ILogger<DistributedLock> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor;
        }

        #region Propriedades

        public string AdminStringConexao
        {
            get
            {
#if DEBUG
                return ObterConnectionAdminMultisoftwareDebug();
#endif

                return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
            }
        }

        public int? ObterPortaHost
        {
            get
            {
                return _httpContextAccessor.HttpContext.Request.Host.Port;
            }
        }


        public string ObterHost
        {
            get
            {
                string host = _httpContextAccessor.HttpContext.Request.Host.Value;

                if (_httpContextAccessor.HttpContext.Request.Host.Port != null && _httpContextAccessor.HttpContext.Request.Host.Port != 80 && _httpContextAccessor.HttpContext.Request.Host.Port != 443 && _httpContextAccessor.HttpContext.Request.Host.Port != 8443)
                    host += ":" + _httpContextAccessor.HttpContext.Request.Host.Port;

                if (_httpContextAccessor.HttpContext.Request.Host.Port == 8443) //Adicionado para validaçao de deploy Bluegren
                    host = host.Replace(":8443", "");

#if DEBUG
                host = ObterArquivoURLDebug().FirstOrDefault();
#endif

                return host;
            }
        }

        public string StringConexao
        {
            get
            {
                string host = ObterHost;
                string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";

                int empresaMultisoftware = CacheProvider.Instance.Get<int>(chaveCacheEmpresa);

                if (empresaMultisoftware == 0)
                    return CriarChache(host);

                string keyCache = $"_STRING_CONEXAO_{empresaMultisoftware}";

                string connectionStringCacheObject = CacheProvider.Instance.Get<string>(keyCache);

                if (!string.IsNullOrEmpty(connectionStringCacheObject))
                    return connectionStringCacheObject;

                return CriarChache(host);
            }
        }

        #endregion

        #region Métodos Privados

        private string CriarChache(string host)
        {
            try
            {
                string stringConexao = "";

                using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao))
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                    if (clienteURLAcesso == null)
                        throw new Exception($"O Host {host} não possui uma configuração");

                    string chaveCacheEmpresa = $"EmpresaMultisoftware{ObterHost}";

                    CacheProvider.Instance.Add(chaveCacheEmpresa, clienteURLAcesso.Cliente.Codigo, TimeSpan.FromHours(12));

                    string keyCache = $"_STRING_CONEXAO_{clienteURLAcesso.Cliente.Codigo}";

                    if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                        stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                    else
                        stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);

#if DEBUG
                    stringConexao = ObterConnectionStringDebug(stringConexao);
#endif
                    Servicos.Log.SetStringConexao(stringConexao);
                    Servicos.Log.SetCliente(clienteURLAcesso.Cliente.NomeFantasia + (clienteURLAcesso.URLHomologacao ? " Homo" : " Produção"));
                    CacheProvider.Instance.Add(keyCache, stringConexao, TimeSpan.FromHours(12));
                }

                return stringConexao;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }
        }

        private string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            if (configuracao.LoginPorAD)
                return $"Application Name=SGT.WebAdmin;Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size=1000;";
            else
                return $"Application Name=SGT.WebAdmin;Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=1000;";
        }

        private List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new ControllerException("Arquivo DebugConfig.txt não localizado.");
        }

        private string ObterConnectionAdminMultisoftwareDebug()
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

        private string ObterConnectionStringDebug(string connectionDefault)
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return connectionDefault;

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            return !string.IsNullOrWhiteSpace((string)configDebug.ConnectionString) ? (string)configDebug.ConnectionString : connectionDefault;
        }

        #endregion

        #region Métodos Públicos

        public void ConfigureFileStorage()
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(AdminStringConexao, ObterHost);
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(StringConexao);
        }

        public void MigrateDatabase(string stringConexao)
        {
            try
            {
                bool? runMigrations = Environment.GetEnvironmentVariable("runMigrations")?.ToBool();
                if (runMigrations.HasValue && !runMigrations.Value)
                    return;

                Migrations.Migrator.MigrateToLatest($"{stringConexao}TrustServerCertificate=true;");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        public int ObterUsuarioDebugPadrao(int codigoPadrao)
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return codigoPadrao;

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            return ((int)configDebug.CodigoUsuario) > 0 ? (int)configDebug.CodigoUsuario : codigoPadrao;
        }

        #endregion
    }
}
