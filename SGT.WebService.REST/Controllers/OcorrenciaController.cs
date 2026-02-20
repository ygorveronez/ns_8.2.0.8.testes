using Dominio.ObjetosDeValor.Embarcador.Ocorrencia;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class OcorrenciaController : BaseService
    {
        #region Construtores
        public OcorrenciaController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos
        /// <summary>
        /// Confirma as ocorrencias integradas
        /// </summary>
        /// <param name="protocolos">Vale Pedagio</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoOcorrencia")]
        public Retorno<bool> ConfirmarIntegracaoOcorrencia(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoOcorrencia(protocolos);
            });
        }

        /// <summary>
        /// Confirma status das integrações de ocorrências
        /// </summary>
        /// <param name="protocolos">Vale Pedagio</param>
        /// <returns></returns>
        [HttpPost("ConfirmarStatusIntegracaoOcorrencia")]
        public Retorno<bool> ConfirmarStatusIntegracaoOcorrencia(RetornoStatusCargaOcorrenciaCTeResult requestDados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarStatusCargaOcorrenciaCTe(requestDados);
            });
        }

        /// <summary>
        /// Retorna todas as Ocorrencias que estão pendentes de integração
        /// </summary>
        /// <param name="quantidade">Quantidade de registros que deseja processar(Opcional). Por padrão ja é 50</param>
        /// <returns></returns>
        [HttpPost("BuscarOcorrenciasPendentesIntegracao")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>> BuscarOcorrenciasPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarOcorrenciasPendentesIntegracao(quantidade);
            });
        }

        [HttpPost("AdicionarOcorrencia")]
        public Retorno<int> AdicionarOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ocorrencia)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AdicionarOcorrencia(ocorrencia);
            });
        }

        /// <summary>
        /// Solicita o cancelamento de uma Ocorrência
        /// </summary>
        /// <param name="protocoloOcorrencia">Protocolo da Ocorrência</param>
        /// <param name="motivoCancelamento">Motivo do cancelamento da Ocorrência</param>
        /// <returns></returns>
        [HttpPost("SolicitarCancelamentoOcorrencia")]
        public Retorno<bool> SolicitarCancelamentoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.SolicitarCancelamentoOcorrencia ocorrenciaCancelamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SolicitarCancelamentoOcorrencia(ocorrenciaCancelamento);
            });
        }

        /// <summary>
        /// Retorna a situacao de uma Ocorrência
        /// </summary>
        /// <param name="protocoloOcorrencia">Protocolo da Ocorrência</param>
        /// <returns></returns>
        [HttpPost("BuscarSituacaoOcorrencia")]
        public Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> BuscarSituacaoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.BuscarSituacaoOcorrencia ocorrenciaSituacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarSituacaoOcorrencia(ocorrenciaSituacao);
            });
        }
        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceOcorrencias;
        }

        #endregion
    }
}
