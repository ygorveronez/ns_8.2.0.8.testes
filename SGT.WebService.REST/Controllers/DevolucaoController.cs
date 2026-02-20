using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para Devoluções.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DevolucaoController : BaseService
    {
        #region Construtores

        public DevolucaoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion Construtores

        #region Métodos Públicos

        /// <summary>
        /// Realizar Integração Pêndencia Financeira.
        /// </summary>
        /// <param name="pendenciaFinanceira">Integrar pendências financeiras do sistema SAP ao Multiembarcador, garantindo que todas as informações necessárias sejam incorporadas automaticamente. </param>
        /// <returns></returns>
        [HttpPost("AdicionarPendenciaFinanceira")]
        public Retorno<bool> AdicionarPendenciaFinanceira(Dominio.ObjetosDeValor.WebService.Devolucao.AdicionarPendenciaFinanceira pendenciaFinanceira)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Devolucao.Devolucao(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                .AdicionarPendenciaFinanceira(pendenciaFinanceira);
            });
        }

        /// <summary>
        /// Receber Atualização de Laudo.
        /// </summary>
        /// <param name="dadosLaudo">Receber Atualização de Laudo.. </param>
        /// <returns></returns>
        [HttpPut("AtualizarLaudo")]
        public Retorno<bool> AtualizarLaudo(Dominio.ObjetosDeValor.WebService.Devolucao.AtualizarLaudo dadosLaudo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Devolucao.Devolucao(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                .AtualizarLaudo(dadosLaudo);
            });
        }

        /// <summary>
        /// Receber Atualização Ocorrência Devolução.
        /// </summary>
        /// <param name="ocorrenciaDevolucao">Receber atualizações de ocorrência. </param>
        /// <returns></returns>
        [HttpPut("AtualizarOcorrenciaDevolucao")]
        public Retorno<bool> AtualizarOcorrenciaDevolucao(Dominio.ObjetosDeValor.WebService.Devolucao.AtualizarOcorrenciaDevolucao ocorrenciaDevolucao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Devolucao.Devolucao(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                .AtualizarOcorrenciaDevolucao(ocorrenciaDevolucao);
            });
        }

        /// Realizar Integração Nota Devolução.
        /// </summary>
        /// <param name="notaDevolucao">Recebimento das notas fiscais de devolução emitidas. </param>
        /// <returns></returns>
        [HttpPost("AdicionarNotaDevolucao")]
        public Retorno<bool> AdicionarNotaDevolucao(Dominio.ObjetosDeValor.WebService.Devolucao.AdicionarNotaDevolucao notaDevolucao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Devolucao.Devolucao(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                .AdicionarNotaDevolucao(notaDevolucao);
            });
        }

        /// <summary>
        /// Finalizar Devolução.
        /// </summary>
        /// <param name="devolucao">Receber dados devolução. </param>
        /// <returns></returns>
        [HttpPut("FinalizarDevolucao")]
        public async Task<Retorno<bool>> FinalizarDevolucao(Dominio.ObjetosDeValor.WebService.Devolucao.FinalizarDevolucao devolucao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                Servicos.WebService.Devolucao.Devolucao servicoDevolucao = new Servicos.WebService.Devolucao.Devolucao(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment));
               return await servicoDevolucao.FinalizarDevolucao(devolucao);
            });
        }


        #endregion Métodos Públicos

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceDevolucao;
        }

        #endregion Métodos Protegidos
    }
}
