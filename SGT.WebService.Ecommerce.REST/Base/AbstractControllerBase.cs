using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace SGT.WebService.Ecommerce.REST.Base
{
    public abstract class AbstractControllerBase : ControllerBase
    {
        #region Construtores

        protected readonly IConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AbstractControllerBase(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            SetarIntegradora();
        }

        #endregion

        #region Propriedades Protegidas

        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;
        protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _cliente = ClienteAcesso.Cliente;

                return _cliente;
            }
        }

        private string _webServiceConsultaCTe;
        protected string WebServiceConsultaCTe
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_webServiceConsultaCTe))
                    _webServiceConsultaCTe = ClienteAcesso.WebServiceConsultaCTe;

                return _webServiceConsultaCTe;
            }
        }

        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _tipoServicoMultisoftware = ClienteAcesso.TipoServicoMultisoftware;

                return _tipoServicoMultisoftware;
            }
        }

        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteURLAcesso;
        protected AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso ClienteAcesso
        {
            get
            {
                if (_clienteURLAcesso == null)
                {
                    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment));
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    _clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.ObterHost(Request, _webHostEnvironment));
                    _tipoServicoMultisoftware = _clienteURLAcesso.TipoServicoMultisoftware;
                    _cliente = _clienteURLAcesso.Cliente;
                    unitOfWork.Dispose();
                }

                return _clienteURLAcesso;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        protected Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado
        {
            get
            {
                if (_auditado == null)
                {
                    _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                        OrigemAuditado = ObterOrigemAuditado()
                    };
                }

                return _auditado;
            }
        }

        protected ActionResult MResult(object value = null, string mensagem = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var resposta = new
            {
                Dados = value,
                Mensagem = mensagem,
                Sucesso = string.IsNullOrWhiteSpace(mensagem)
            };

            return new ObjectResult(resposta)
            {
                StatusCode = (int)statusCode
            };
        }

        protected ActionResult MResult(string mensagem = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ObjectResult(mensagem)
            {
                StatusCode = (int)statusCode
            };
        }

        #endregion

        #region Métodos Privados

        private void SetarIntegradora()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(_httpContextAccessor.HttpContext.Request, _memoryCache, _configuration, _webHostEnvironment), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            string ipOrigem = _httpContextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string token = _httpContextAccessor.HttpContext.User.FindFirst("Token")?.Value ?? string.Empty;

            Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
            Dominio.Entidades.WebService.Integradora integradora = repIntegradora.BuscarPorToken(token);

            if (_auditado == null)
            {
                _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                    OrigemAuditado = ObterOrigemAuditado()
                };
            }

            _auditado.Integradora = integradora;
            _auditado.IP = Utilidades.String.Left(ipOrigem, 50);
        }

        #endregion

        #region Métodos Protegidos Abstratos

        protected abstract OrigemAuditado ObterOrigemAuditado();

        #endregion
    }
}
