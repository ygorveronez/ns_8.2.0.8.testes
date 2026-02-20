using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.Rest.Frete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;


namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para Gestão Contrato Frete.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ContratoFreteTerceiroController : BaseService
    {
        #region Construtores
        public ContratoFreteTerceiroController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Obter os contratos frete pendente de integração.
        /// </summary>
        /// <param name="dadosRequest">Paramtros de consulta</param>
        /// <returns></returns>
        [HttpPost("BuscarContratosFretePendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>> BuscarContratosFretePendentesIntegracao(RequestContratoFrete dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarContratosFretePendentesIntegracao(dadosRequest.DataInicial, dadosRequest.DataFinal, dadosRequest.Inicio, dadosRequest.QuantidadeRegistros);
            });
        }

        /// <summary>
        /// Confimar Integração Contrato Frete
        /// </summary>
        /// <param name="protocolos">Protocolos para confirmar integração</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoContratoFrete")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoContratoFrete(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoContratoFrete(protocolos);
            });
        }
        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebSContratoFreteTerceiro;
        }

        #endregion

    }
}
