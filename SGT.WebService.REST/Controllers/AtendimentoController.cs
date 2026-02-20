using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para Atendimentos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AtendimentoController : BaseService
    {
        #region MyRegion

        private readonly IAdicionarRequestAssincrono _servicoAdicionarRequestAssincrono;

        #endregion

        #region Construtores

        public AtendimentoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IAdicionarRequestAssincrono servicoAdicionarRequestAssincrono) : base(memoryCache, configuration, webHostEnvironment)
        {
            _servicoAdicionarRequestAssincrono = servicoAdicionarRequestAssincrono;

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Realizar abertura do atendimento.
        /// </summary>
        /// <param name="adicionarAtendimento">Atendimento que será adicionado.</param>
        /// <returns></returns>
        [HttpPost("AdicionarAtendimento")]
        public Retorno<bool> AdicionarAtendimento(Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento adicionarAtendimento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<int> retorno = new Servicos.WebService.Atendimento.Atendimento(unitOfWork, TipoServicoMultisoftware, Auditado, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AdicionarAtendimento(adicionarAtendimento, default).GetAwaiter().GetResult();

                return new Retorno<bool>
                {
                    Status = retorno.Status,
                    Mensagem = retorno.Mensagem,
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Objeto = retorno.Objeto > 0
                };
            });
        }

        /// <summary>
        /// Realizar abertura de atendimentos em lote.
        /// </summary>
        /// <param name="adicionarAtendimentos">Atendimentos que serão adicionados.</param>
        /// <returns></returns>
        [HttpPost("AdicionarAtendimentoEmLote")]
        public async Task<Retorno<RetornoAdicionarRequestAssincrono>> AdicionarAtendimentoEmLote(List<Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento> adicionarAtendimentos)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                // ⚡ Nova assinatura - apenas tipos de etapas
                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.QuebrarRequest,
                    TipoEtapaTarefa.AdicionarAtendimento,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                RetornoAdicionarRequestAssincrono retornoAdicionarRequestAssincrono = await _servicoAdicionarRequestAssincrono.SalvarLoteAsync(adicionarAtendimentos, TipoRequest.AdicionarAtendimentoEmLote, etapas, cancellationToken, integradora.Codigo);

                return Retorno<RetornoAdicionarRequestAssincrono>.CriarRetornoSucesso(retornoAdicionarRequestAssincrono, "Lote adicionado para processamento.");
            });
        }

        #endregion Métodos Públicos

        #region Metodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceAtendimento;
        }

        #endregion Metodos Protegidos
    }
}
