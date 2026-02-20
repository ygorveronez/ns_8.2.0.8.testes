using Microsoft.Extensions.Caching.Memory;

namespace SGT.WebService.Ecommerce.REST.Base
{
    public class Conexao
    {
        #region Métodos Globais

        public static string AdminStringConexao(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            string keyCacheStringConexaoAdmin = "AdminMultisoftwareStringConexao";

            object objCacheStringConexaoAdmin = memoryCache.Get(keyCacheStringConexaoAdmin);

            if (objCacheStringConexaoAdmin != null)
                return objCacheStringConexaoAdmin.ToString();

            string stringConexaoAdminMultisoftware = string.Empty;

#if DEBUG
            stringConexaoAdminMultisoftware = ObterConnectionAdminMultisoftwareDebug(webHostEnvironment);
#else
            stringConexaoAdminMultisoftware = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
#endif

            memoryCache.GetOrCreate(keyCacheStringConexaoAdmin, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return stringConexaoAdminMultisoftware;
            });

            return stringConexaoAdminMultisoftware;
        }

        public static string StringConexao(HttpRequest request, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            string host = ObterHost(request, webHostEnvironment);

            return ObterStringConexao(host, memoryCache, configuration, webHostEnvironment);
        }

        public static string ObterHost(HttpRequest request, IWebHostEnvironment webHostEnvironment)
        {
            string host = request.Host.ToString();
#if DEBUG
            host = ObterArquivoURLDebug(webHostEnvironment).FirstOrDefault();
#endif

            return host;
        }

        public static string ObterStringConexao(string host, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            string keyCacheEmpresa = $"EmpresaMultisoftware{host}";
            object objCacheEmpresa = memoryCache.Get(keyCacheEmpresa);

            int empresaMultisoftware = objCacheEmpresa != null ? int.Parse(objCacheEmpresa.ToString()) : 0;

            if (empresaMultisoftware == 0)
                return CriarChache(host, memoryCache, configuration, webHostEnvironment);

            string keyCacheStringConexao = $"_STRING_CONEXAO_{empresaMultisoftware}";

            object objCacheStringConexao = memoryCache.Get(keyCacheStringConexao);

            if (objCacheStringConexao != null)
                return objCacheStringConexao.ToString();

            return CriarChache(host, memoryCache, configuration, webHostEnvironment);
        }

        #endregion

        #region Métodos Privados

        private static string CriarChache(string host, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            string stringConexao = "";

            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao(memoryCache, configuration, webHostEnvironment)))
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                if (clienteURLAcesso == null)
                    throw new Exception($"O host {host} não possui uma configuração.");

                string keyCacheEmpresa = $"EmpresaMultisoftware{host}";

                memoryCache.GetOrCreate(keyCacheEmpresa, cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return clienteURLAcesso.Cliente.Codigo;
                });

                string keyCacheStringConexao = $"_STRING_CONEXAO_{clienteURLAcesso.Cliente.Codigo}";

                if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                else
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);

#if DEBUG
                stringConexao = ObterConnectionStringDebug(stringConexao, webHostEnvironment);
#endif
                memoryCache.GetOrCreate(keyCacheStringConexao, cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return stringConexao;
                });
            }

            return stringConexao;
        }

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            if (configuracao.LoginPorAD)
                return $"Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size=600;";
            else
                return $"Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=600;";
        }

        private static List<string> ObterArquivoURLDebug(IWebHostEnvironment webHostEnvironment)
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(webHostEnvironment.ContentRootPath, "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new Exception("Arquivo DebugConfig.txt não localizado.");
        }

        private static string ObterConnectionAdminMultisoftwareDebug(IWebHostEnvironment webHostEnvironment)
        {
            List<string> arquivo = ObterArquivoURLDebug(webHostEnvironment);

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                throw new Exception("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            string connectionString = (string)configDebug.AdminMultisoftware;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("String de conexão do AdminMultisoftware não encontrada.");

            return connectionString;
        }

        private static string ObterConnectionStringDebug(string connectionDefault, IWebHostEnvironment webHostEnvironment)
        {
            List<string> arquivo = ObterArquivoURLDebug(webHostEnvironment);

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return connectionDefault;

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            return !string.IsNullOrWhiteSpace((string)configDebug.ConnectionString) ? (string)configDebug.ConnectionString : connectionDefault;
        }

        #endregion
    }
}