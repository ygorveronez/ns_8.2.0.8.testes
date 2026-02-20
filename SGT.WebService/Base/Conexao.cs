using Dominio.Excecoes.Embarcador;
using Servicos.Embarcador.EmissorDocumento;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services.Cache;

namespace SGT.WebService
{
    public class Conexao(IServiceProvider serviceProvider)
    {
        
        public static Conexao createInstance(IServiceProvider serviceProvider)
        {
            return new Conexao(serviceProvider);
        }
        private IHttpContextAccessor httpContextAcessor => serviceProvider.GetRequiredService<IHttpContextAccessor>();
        
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

        public string ObterHost
        {
            get
            {
                string host = httpContextAcessor.HttpContext.Request.Host.Value;

                if (httpContextAcessor.HttpContext.Request.Host.Port != null && httpContextAcessor.HttpContext.Request.Host.Port != 80 && httpContextAcessor.HttpContext.Request.Host.Port != 443)
                    host += ":" + httpContextAcessor.HttpContext.Request.Host.Port;

                if (httpContextAcessor.HttpContext.Request.Host.Port == 8443) //Adicionado para validaçao de deploy Bluegren
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
                return ObterStringConexao(host);
            }
        }

        public string ObterStringConexao(string host)
        {
            string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";
            int empresaMultisoftware = CacheProvider.Instance.Get<int>(chaveCacheEmpresa);

            if (empresaMultisoftware == 0)
                return CriarChache(host);

            string keyCache = $"_STRING_CONEXAO_{empresaMultisoftware}";
            var objetoCache = CacheProvider.Instance.Get<string>(keyCache);

            if (!string.IsNullOrEmpty(objetoCache))
                return objetoCache;

            return CriarChache(host);
        }

        public void ConfigureFileStorage()
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(AdminStringConexao, ObterHost);
            EmissorDocumentoService.ConfigureApplicationEmissorDocumento(StringConexao);
        }

        #endregion

        #region Métodos Privados

        private string CriarChache(string host)
        {
            string stringConexao = "";

            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao))
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                if (clienteURLAcesso == null)
                    throw new Exception($"O Host {host} não possui uma configuração");

                string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";
                CacheProvider.Instance.Add(chaveCacheEmpresa, clienteURLAcesso.Cliente.Codigo, TimeSpan.FromHours(1));
                string keyCache = $"_STRING_CONEXAO_{clienteURLAcesso.Cliente.Codigo}";

                if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                else
                    stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);

#if DEBUG
                stringConexao = ObterConnectionStringDebug(stringConexao);
#endif
                LoadConfigurations(stringConexao);
                CacheProvider.Instance.Add(keyCache, stringConexao, TimeSpan.FromHours(1));

                Servicos.Log.SetStringConexao(stringConexao);
                Servicos.Log.SetCliente(clienteURLAcesso.Cliente.NomeFantasia + (clienteURLAcesso.URLHomologacao ? " Homo" : " Produção"));

            }

            return stringConexao;
        }

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            if (configuracao.LoginPorAD)
                return $"Application Name=SGT.WebService;Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size=600;";
            else
                return $"Application Name=SGT.WebService;Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=600;";
        }

        private static List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new ControllerException("Arquivo DebugConfig.txt não localizado.");
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

        private static void LoadConfigurations(string stringConexao)
        {
#if !DEBUG
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);
            }
            finally
            {
                unitOfWork.Dispose();
            }
#endif
        }

        #endregion
    }
}
