using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST
{
    public class Conexao
    {
        #region Métodos Globais

        public static string AdminStringConexao(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
#if DEBUG
            return ObterConnectionAdminMultisoftwareDebug(webHostEnvironment);
#endif
            return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
        }

        public static string StringConexao(HttpRequest request, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, int maxpoolsize = 600)
        {
            string host = ObterHost(request, webHostEnvironment);

            return ObterStringConexao(host, memoryCache, configuration, webHostEnvironment, maxpoolsize);
        }

        public static string ObterHost(HttpRequest request, IWebHostEnvironment webHostEnvironment)
        {
            string host = request.Host.ToString();

            if (request.Host.Port == 8443) //Adicionado para validaçao de deploy Bluegren
                host = host.Replace(":8443", "");

#if DEBUG
            host = ObterArquivoURLDebug(webHostEnvironment).FirstOrDefault();
#endif

            return host;
        }

        public static string ObterStringConexao(string host, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, int maxpoolsize)
        {
            string keyCacheEmpresa = $"EmpresaMultisoftware{host}";
            object objCacheEmpresa = memoryCache.Get(keyCacheEmpresa);

            int empresaMultisoftware = objCacheEmpresa != null ? int.Parse(objCacheEmpresa.ToString()) : 0;

            if (empresaMultisoftware == 0)
                return CriarChache(host, memoryCache, configuration, webHostEnvironment,maxpoolsize);

            string keyCacheStringConexao = $"_STRING_CONEXAO_{empresaMultisoftware}";

            object objCacheStringConexao = memoryCache.Get(keyCacheStringConexao);

            if (objCacheStringConexao != null)
                return objCacheStringConexao.ToString();

            return CriarChache(host, memoryCache, configuration, webHostEnvironment, maxpoolsize);
        }

        public static void ConfigureFileStorage(HttpRequest request, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(AdminStringConexao(memoryCache, configuration, webHostEnvironment), ObterHost(request, webHostEnvironment));
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(StringConexao(request, memoryCache, configuration, webHostEnvironment));
        }

        #endregion

        #region Métodos Privados

        private static string CriarChache(string host, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, int maxpoolsize)
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
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao, maxpoolsize);
                else
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao,maxpoolsize);

#if DEBUG
                stringConexao = ObterConnectionStringDebug(stringConexao, webHostEnvironment);
#endif
                Servicos.Log.SetStringConexao(stringConexao);
                Servicos.Log.SetCliente(clienteURLAcesso.Cliente.NomeFantasia + (clienteURLAcesso.URLHomologacao ? " Homo" : " Produção"));
                memoryCache.GetOrCreate(keyCacheStringConexao, cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return stringConexao;
                });
            }

            return stringConexao;
        }

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao, int maxpoolsize)
        {
            if (configuracao.LoginPorAD)
                return $"Application Name=SGT.WebService.REST;Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size={maxpoolsize};";
            else
                return $"Application Name=SGT.WebService.REST;Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size={maxpoolsize};";
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

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            string connectionString = (string)configDebug.AdminMultisoftware;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ControllerException("String de conexão do AdminMultisoftware não encontrada.");

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
