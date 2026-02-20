using Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Logistica.JanelaCarregamento;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class JanelaCarregamentoController : BaseService
    {
        #region Constutores

        public JanelaCarregamentoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Métodos Públicos

        /// <summary>
        ///  Endpoint Responsavel por controlar a liberação das cargas para os transportadores
        /// </summary>
        /// <param name="controleLiberacaoTransportadores">Dados para liberação das cargas para os transportadores</param>
        /// <returns></returns>
        [HttpPost("ControlarLiberacaoTransportadores")]
        public Retorno<bool> ControlarLiberacaoTransportadores(ControleLiberacaoTransportadores controleLiberacaoTransportadores)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Logistica.JanelaCarregamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ControlarLiberacaoTransportadores(controleLiberacaoTransportadores);
            });
        }


        /// <summary>
        ///  Endpoint Responsable para informar etapa, do fluxo de patio
        /// </summary>
        /// <param name="dadosRequest">Dados para informar Etapa de fluxo de patio</param>
        /// <returns></returns>
        [HttpPost("InformarEtapaFluxoPatio")]
        public Retorno<bool> InformarEtapaFluxoPatio(InformarEtapaFluxoPatio dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.JanelaCarregamento.JanelaCarregamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).InformarEtapaFluxoPatio(dadosRequest);
            });
        }


        /// <summary>
        ///  Endpoint Responsavel por informar etapa do fluxo de patio por placa do veículo
        /// </summary>
        /// <param name="corpo">Placa do veículo, número da etapa e data a constar.</param>
        /// <returns></returns>
        [HttpPost("InformarEtapaFluxoPatioPorPlaca")]
        public Retorno<bool> InformarEtapaFluxoPatioPorPlaca(Dominio.ObjetosDeValor.WebService.Carga.AvancoFluxoPatioPorPlaca corpo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.JanelaCarregamento.JanelaCarregamento(
                    unitOfWork, 
                    TipoServicoMultisoftware, 
                    Cliente, 
                    Auditado, 
                    ClienteAcesso, 
                    Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)
                ).InformarEtapaFluxoPatioPorPlaca(corpo);
            });
        }


        /// <summary>
        ///  Endpoint Responsavel por avancar fluxo de patio 
        /// </summary>
        /// <param name="integracaoEventosFluxoPatio">Dados para avancar fluxo de patio </param>
        /// <returns></returns>
        [HttpPost("EventosFluxoPatio")]
        public Retorno<bool> EventosFluxoPatio(Dominio.ObjetosDeValor.WebService.GestaoPatio.IntegracaoEventosFluxoPatio integracaoEventosFluxoPatio)
        {
            Retorno<bool> retorno = ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<bool> retorno = new Retorno<bool>();
                retorno = new Servicos.WebService.Logistica.JanelaCarregamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).EventosFluxoPatio(integracaoEventosFluxoPatio);
                if (retorno.CodigoMensagem > 200)
                    Response.StatusCode = retorno.CodigoMensagem;

                return retorno;
            });

            if (retorno.CodigoMensagem > 200)
                Response.StatusCode = retorno.CodigoMensagem;
            return retorno;

        }
        #endregion

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceJanelaCarregamento;
        }
    }
}
