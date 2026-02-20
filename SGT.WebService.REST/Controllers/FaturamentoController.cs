using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FaturamentoController : BaseService
    {
        #region Construtores
        public FaturamentoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos
        /// <summary>
        /// Confirma os Documentos de pagamento integrados
        /// </summary>
        /// <param name="protocolo">Protocolos dos documento para confirmar</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoDocumentoFaturamento")]
        public Retorno<bool> ConfirmarIntegracaoDocumentoFaturamento(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Faturamento.Faturamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoDocumentoFaturamento(protocolo);
            });
        }

        /// <summary>
        /// Busca Documentos de pagamento pendentes de integração
        /// </summary>
        /// <param name="request">Dados para consulta de documento pendentes de integração</param>
        /// <returns></returns>
        [HttpPost("BuscarDocumentosPagamentoLiberado")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>> BuscarDocumentosPagamentoLiberado(Dominio.ObjetosDeValor.WebService.Faturamento.RequestDocumentoPagamento request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Faturamento.Faturamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDocumentosPagamentoLiberado(request.TipoDocumentoRetorno, request.Inicio, request.Limite);
            });
        }

        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceFaturamento;
        }

        #endregion
    }
}
