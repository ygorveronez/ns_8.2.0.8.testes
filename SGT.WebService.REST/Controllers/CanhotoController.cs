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
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CanhotoController : BaseService
    {
        #region Propriedades Privadas Somente Leitura

        private readonly IAdicionarRequestAssincrono _servicoAdicionarRequestAssincrono;

        #endregion Propriedades Privadas Somente Leitura

        #region Construtores

        public CanhotoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IAdicionarRequestAssincrono servicoAdicionarRequestAssincrono) : base(memoryCache, configuration, webHostEnvironment)
        {
            _servicoAdicionarRequestAssincrono = servicoAdicionarRequestAssincrono;
        }

        #endregion

        #region Metodos Publicos
        /// <summary>
        ///  Endpoint responsavel por retornar os canhotos que foram digitalizados e estão pendentes de integração.
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("BuscarCanhotosNotasFiscaisDigitalizados")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosNotasFiscaisDigitalizados(RequestPaginacao dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCanhotosNotasFiscaisDigitalizados(dadosRequest, integradora);
            });
        }
        /// <summary>
        ///  Endpoint responsavel por retornar os canhotos que estão no status Ag. Aprovação e Digitalizados.
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("BuscarCanhotosDigitalizadoseAgAprovacao")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosDigitalizadoseAgAprovacao(RequestPaginacao dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                .BuscarCanhotosDigitalizadoseAgAprovacao(dadosRequest, integradora);
            });
        }

        /// <summary>
        ///  Endpoint Responsavel por confirmar Integração dos canhotos Digitalizados
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais")]
        public Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(protocolos);
            });
        }

        /// <summary>
        ///  Endpoint Responsavel por confirmar Integração CT-es baseado nos canhotos Digitalizados
        /// </summary>
        /// <param name="retornoCanhotoEntrega">Dados para confirmar integração</param>
        /// <returns></returns>
        [HttpPost("RetornoCanhotoEntrega")]
        public Retorno<bool> RetornoCanhotoEntrega(Dominio.ObjetosDeValor.Embarcador.Canhoto.RetornoCanhotoEntrega retornoCanhotoEntrega)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoCanhotoEntrega(retornoCanhotoEntrega);
            });
        }

        /// <summary>
        /// Endpoint Responsavel por enviar a digitalização do canhoto.
        /// </summary>
        /// <param name="canhotoDigitalizacao">canhoto que será digitalizado.</param>
        /// <returns></returns>
        [HttpPost("EnviarDigitalizacaoCanhoto")]
        public async Task<Retorno<int>> EnviarDigitalizacaoCanhoto(Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoDigitalizacao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, "").EnviarDigitalizacaoCanhotoAsync(canhotoDigitalizacao, integradora, cancellationToken);

            });
        }

        /// <summary>
        /// Endpoint Responsavel por enviar a digitalização do canhoto em lote.
        /// </summary>
        /// <param name="canhotosDigitalizacao">Lista de canhotos que serão digitalizados.</param>
        /// <returns></returns>
        [HttpPost("EnviarDigitalizacaoCanhotoEmLote")]
        public async Task<Retorno<RetornoAdicionarRequestAssincrono>> EnviarDigitalizacaoCanhotoEmLote(List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto> canhotosDigitalizacao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                // ⚡ Nova assinatura - apenas tipos de etapas
                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.QuebrarRequest,
                    TipoEtapaTarefa.EnviarDigitalizacaoCanhoto,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                RetornoAdicionarRequestAssincrono retornoAdicionarRequestAssincrono = await _servicoAdicionarRequestAssincrono.SalvarLoteAsync(canhotosDigitalizacao, TipoRequest.EnviarDigitalizacaoCanhotoEmLote, etapas, cancellationToken, integradora.Codigo);

                return Retorno<RetornoAdicionarRequestAssincrono>.CriarRetornoSucesso(retornoAdicionarRequestAssincrono, "Lote adicionado para processamento.");
            });
        }

        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceCanhotos;
        }

        #endregion
    }
}
