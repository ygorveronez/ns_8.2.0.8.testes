using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Carga;
using Dominio.ObjetosDeValor.WebService.Rest;
using Dominio.ObjetosDeValor.WebService.Rest.Pedidos;
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
    /// API para cadastro e manutenção de pedidos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PedidosController : BaseService
    {
        private readonly IAdicionarRequestAssincrono _servicoAdicionarRequestAssincrono;

        #region Construtores

        public PedidosController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IAdicionarRequestAssincrono servicoAdicionarRequestAssincrono) : base(memoryCache, configuration, webHostEnvironment)
        {
            _servicoAdicionarRequestAssincrono = servicoAdicionarRequestAssincrono;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adicionar ou atualizar Pedido.
        /// </summary>
        /// <param name="cargaIntegracao">Pedido que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("AdicionarPedido")]
        public async Task<Retorno<Protocolos>> AdicionarPedidoAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            return await ProcessarRequisicaoAsync((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AdicionarPedidoAsync(cargaIntegracao, false, cancellationToken);
            });
        }

        /// <summary>
        /// Adicionar ou atualizar Pedidos em Lote.
        /// </summary>
        /// <param name="cargasIntegracao">Lista de pedidos que serão adicionados ou atualizados.</param>
        /// <returns></returns>
        [HttpPost("AdicionarPedidoEmLote")]
        public async Task<Retorno<RetornoAdicionarRequestAssincrono>> AdicionarPedidoEmLote(List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.QuebrarRequest,
                    TipoEtapaTarefa.AdicionarPedido,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                RetornoAdicionarRequestAssincrono retornoAdicionarRequestAssincrono = await _servicoAdicionarRequestAssincrono.SalvarLoteAsync(cargasIntegracao, TipoRequest.AdicionarPedidoEmLote, etapas, cancellationToken, integradora.Codigo);

                return Retorno<RetornoAdicionarRequestAssincrono>.CriarRetornoSucesso(retornoAdicionarRequestAssincrono, "Lote adicionado para processamento.");
            });
        }

        /// <summary>
        /// Atualizar Pedido.
        /// </summary>
        /// <param name="Dados">Pedido que será Atualizado.</param>
        /// <returns></returns>
        [HttpPost("AtualizarPedido")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarPedido(ParametroParaAtualizarCarga Dados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarCargaPadrao(Dados.CargaIntegracao, Dados.Protocolo, unitOfWork.StringConexao, false);
            });
        }

        /// <summary>
        /// Enviar Pacote
        /// </summary>
        /// <param name="Pacote">Enviar um pacote</param>
        /// <returns></returns>
        [HttpPost("EnviarPacote")]
        public Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>> EnviarPacote(Pacote Pacote)
        {
            return ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment), string.Empty, cancellationToken).EnviarPacoteAsync(Pacote);
            });
        }

        /// <summary>
        /// Atualizar Produtos do Pedido.
        /// </summary>
        /// <param name="atualizacaoPedidoProduto">Produtos do pedido que serão atualizados.</param>
        /// <returns></returns>
        [HttpPost("AtualizarPedidoProduto")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarPedidoProduto(Dominio.ObjetosDeValor.WebService.Pedido.AtualizacaoPedidoProduto atualizacaoPedidoProduto)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarPedidoProduto(atualizacaoPedidoProduto);
            });
        }

        /// <summary>
        /// Retorna os Tipo de Operações que ainda não foram integrados.
        /// </summary>
        /// <param name="quantidade">Informar a quantidade de registros que deve ser retornados, Por padrão é 50 </param>
        /// <returns></returns>
        [HttpPost("BuscarTiposOperacoesPendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscarTiposOperacoesPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarTiposOperacoesPendentesIntegracao(quantidade);
            });
        }

        /// <summary>
        /// Confirma os tipo de operação ja integrados.
        /// </summary>
        /// <param name="listaProtocolos">Lista com os protocolos dos tipo de operações que desejam ser confirmados </param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoTiposOperacoes")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoTiposOperacoes(List<int> listaProtocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoTiposOperacoes(listaProtocolos);
            });
        }

        /// <summary>
        /// Busca de pedido pelo protocolo.
        /// </summary>
        /// <param name="protocolo">Protocolo para pesquisa de pedidos</param>
        /// <returns></returns>
        [HttpPost("BuscarPedidoPorProtocolo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarPedidoPorProtocolo(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarPedidoService(protocolo);
            });
        }

        /// <summary>
        /// Busca de pedidos com agendamento por filial.
        /// </summary>
        /// <param name="obterAgendamentos">Dados para a busca dos agendamentos</param>
        /// <returns></returns>
        [HttpPost("ObterAgendamentos")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>> ObterAgendamentos(Dominio.ObjetosDeValor.WebService.Pedido.ObterAgendamentos obterAgendamentos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterAgendamentos(obterAgendamentos);
            });
        }

        /// <summary>
        /// Libera carga sem NFe.
        /// </summary>
        /// <param name="protocolo">Dados para a busca da carga</param>
        /// <returns></returns>
        [HttpPost("LiberarEmissaoSemNFe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> LiberarEmissaoSemNFe(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).LiberarEmissaoSemNFe(protocolo);
            });
        }

        /// <summary>
        /// Libera carga sem NFe.
        /// </summary>
        /// <param name="alterarSituacaoComercialPedido">Dados para a busca do pedido e situação comercial/param>
        /// <returns></returns>
        [HttpPost("AlterarSituacaoComercialPedido")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AlterarSituacaoComercialPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlterarSituacaoComercialPedido alterarSituacaoComercialPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AlterarSituacaoComercialPedido(alterarSituacaoComercialPedido);
            });
        }

        /// <summary>
        /// Consultar situação do pedido.
        /// </summary>
        /// <param name="protocoloIntegracaoPedido">Dados para buscar a situação do pedido/param>
        /// <returns></returns>
        [HttpPost("ConsultarSituacaoPedido")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido> ConsultarSituacaoPedido(int protocoloIntegracaoPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConsultarSituacaoPedido(protocoloIntegracaoPedido);
            });
        }


        /// <summary>
        /// Ajustar as datas do pedido.
        /// </summary>
        /// <param name="atualizarDatasPedido">Altera as datas do pedido/param>
        /// <returns></returns>
        [HttpPost("AjustarDatasDoPedido")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AjustarDatasDoPedido(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AjustarDatasDoPedido(atualizarDatasPedido);
            });
        }
        #endregion

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServicePedidos;
        }

        #endregion
    }
}
