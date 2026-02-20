using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using Infrastructure.Services.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Database
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private static Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo _configuracaoMongo;

        private string _host;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? _tipoServicoMultisoftware;
        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente? _cliente;
        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado? _auditado;

        public TenantService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            if (_configuracaoMongo == null)
            {
                Repositorio.UnitOfWork UnitOfWork = new Repositorio.UnitOfWork(StringConexao());

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo repositorioMongo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo(UnitOfWork);
                _configuracaoMongo = repositorioMongo.BuscarConfiguracaoPadrao();

#if DEBUG
                _configuracaoMongo.Banco = "gruposc-teste";
#endif

                UnitOfWork.Dispose();
            }
        }

        #region Métodos Globais

        public MongoUrlBuilder ObterMongoDbConfiguracao()
        {
            if (_configuracaoMongo == null)
                return null;

            return _configuracaoMongo.MongoUrl;
        }

        public string AdminStringConexao()
        {
#if DEBUG
            return ObterConnectionAdminMultisoftwareDebug();
#endif
            return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
        }

        public string StringConexao()
        {
            string host = ObterHost();

            return ObterStringConexao(host);
        }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware ObterTipoServicoMultisoftware()
        {
            if (!_tipoServicoMultisoftware.HasValue)
            {
                using AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao());

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_host);

                if (clienteURLAcesso == null)
                    throw new Exception($"O Host {_host} não possui uma configuração");

                _tipoServicoMultisoftware = clienteURLAcesso.TipoServicoMultisoftware;
            }

            return _tipoServicoMultisoftware.Value;
        }

        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ObterCliente()
        {
            if (_cliente == null)
            {
                string host = ObterHost();

                using AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao());

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                if (clienteURLAcesso == null)
                    throw new Exception($"O Host {host} não possui uma configuração");

                _cliente = clienteURLAcesso.Cliente;
            }

            return _cliente;
        }

        public Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado ObterAuditado(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado origemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema)
        {
            _auditado ??= new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                OrigemAuditado = origemAuditado
            };

            return _auditado;
        }

        #endregion  Métodos Globais

        #region Métodos Privados

        private string ObterHost()
        {
            if (!string.IsNullOrWhiteSpace(_host))
                return _host;

            if (_httpContextAccessor?.HttpContext?.Request?.Host != null)
            {
                _host = _httpContextAccessor.HttpContext.Request.Host.Value;

                if (_httpContextAccessor.HttpContext.Request.Host.Port != null && _httpContextAccessor.HttpContext.Request.Host.Port != 80 && _httpContextAccessor.HttpContext.Request.Host.Port != 443)
                    _host += ":" + _httpContextAccessor.HttpContext.Request.Host.Port;
            }
            else
                _host = _configuration.GetSection("Host").Value;

#if DEBUG
            _host = ObterArquivoURLDebug().FirstOrDefault();
#endif

            return _host;
        }

        private string ObterStringConexao(string host)
        {
            string keyCacheEmpresa = $"EmpresaMultisoftware{host}";
            int empresaMultisoftware = CacheProvider.Instance.Get<int>(keyCacheEmpresa);

            if (empresaMultisoftware == 0)
                return CriarChache(host);

            string keyCacheStringConexao = $"_STRING_CONEXAO_{empresaMultisoftware}";

            string objCacheStringConexao = CacheProvider.Instance.Get<string>(keyCacheStringConexao);

            if (!string.IsNullOrEmpty(objCacheStringConexao))
                return objCacheStringConexao;

            return CriarChache(host);
        }

        private string CriarChache(string host)
        {
            try
            {
                string stringConexao = "";

                using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao()))
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                    if (clienteURLAcesso == null)
                        throw new Exception($"O Host {host} não possui uma configuração");

                    string chaveCacheEmpresa = $"EmpresaMultisoftware{host}";

                    CacheProvider.Instance.Add(chaveCacheEmpresa, clienteURLAcesso.Cliente.Codigo, TimeSpan.FromHours(12));

                    string keyCache = $"_STRING_CONEXAO_{clienteURLAcesso.Cliente.Codigo}";

                    if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                        stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                    else
                        stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);

#if DEBUG
                    stringConexao = ObterConnectionStringDebug(stringConexao);
#endif

                    CacheProvider.Instance.Add(keyCache, stringConexao, TimeSpan.FromHours(12));
                }

                return stringConexao;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw excecao;
            }
        }

        private string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            if (configuracao.LoginPorAD)
                return $"Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size=1000;";
            else
                return $"Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=1000;";
        }

        private List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

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
    }
}