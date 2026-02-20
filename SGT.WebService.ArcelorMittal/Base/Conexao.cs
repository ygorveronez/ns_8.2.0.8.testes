using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SGT.WebService.ArcelorMittal
{
    public class Conexao
    {
        #region Propriedades

        public static string AdminStringConexao
        {
            get
            {
#if DEBUG
                return ObterConnectionAdminMultisoftwareDebug();
#endif

                return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
            }
        }

        public static string ObterHost
        {
            get
            {
                string host = HttpContext.Current.Request.Url.Host;

                if (HttpContext.Current.Request.Url.Port != 80 && HttpContext.Current.Request.Url.Port != 443)
                    host += ":" + HttpContext.Current.Request.Url.Port;

#if DEBUG
                host = ObterArquivoURLDebug().FirstOrDefault();
#endif

                return host;
            }
        }

        public static string StringConexao
        {
            get
            {
                string host = ObterHost;
                //string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";
                //int empresaMultisoftware = HttpContext.Current.Cache.Get(chaveCacheEmpresa) != null ? int.Parse(HttpContext.Current.Cache.Get(chaveCacheEmpresa).ToString()) : 0;

                //if (empresaMultisoftware == 0)
                //    return CriarChache(host);

                //string keyCache = $"_STRING_CONEXAO_{empresaMultisoftware}";

                //if (HttpContext.Current.Cache.Get(keyCache) != null)
                //    return HttpContext.Current.Cache.Get(keyCache).ToString();

                return ObterStringConexao(host);
            }
        }

        public static string ObterStringConexao(string host)
        {
            string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";
            int empresaMultisoftware = HttpContext.Current.Cache.Get(chaveCacheEmpresa) != null ? int.Parse(HttpContext.Current.Cache.Get(chaveCacheEmpresa).ToString()) : 0;

            if (empresaMultisoftware == 0)
                return CriarChache(host);

            string keyCache = $"_STRING_CONEXAO_{empresaMultisoftware}";

            if (HttpContext.Current.Cache.Get(keyCache) != null)
                return HttpContext.Current.Cache.Get(keyCache).ToString();

            return CriarChache(host);
        }

        #endregion

        #region Métodos Privados

        private static string CriarChache(string host)
        {
            string stringConexao = "";

            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao))
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                if (clienteURLAcesso == null)
                    throw new Exception($"O Host {host} não possui uma configuração");

                string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";
                HttpContext.Current.Cache.Add(chaveCacheEmpresa, clienteURLAcesso.Cliente.Codigo, null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                string keyCache = $"_STRING_CONEXAO_{clienteURLAcesso.Cliente.Codigo}";

                if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                else
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);

#if DEBUG
                stringConexao = ObterConnectionStringDebug(stringConexao);
#endif

                HttpContext.Current.Cache.Add(keyCache, stringConexao, null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
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

        private static List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new Exception("Arquivo DebugConfig.txt não localizado.");
        }

        private static string ObterConnectionAdminMultisoftwareDebug()
        {
            List<string> arquivo = ObterArquivoURLDebug();

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

        #endregion
    }
}
