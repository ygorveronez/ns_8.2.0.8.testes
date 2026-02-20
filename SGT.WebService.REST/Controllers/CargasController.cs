using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Carga;
using Dominio.ObjetosDeValor.WebService.Rest;
using Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de cargas.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CargasController : BaseService
    {

        private readonly IAdicionarRequestAssincrono _adicionarRequestAssincrono;

        #region Construtores

        public CargasController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IAdicionarRequestAssincrono adicionarRequestAssincrono) : base(memoryCache, configuration, webHostEnvironment)
        {
            _adicionarRequestAssincrono = adicionarRequestAssincrono;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Obter os tipos de operação disponíveis para um CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ do cliente.</param>
        /// <returns></returns>
        [HttpGet("BuscaTiposDeOperacaoPorCNPJ/{cnpj}")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscaTiposDeOperacaoPorCNPJ(string cnpj)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.TipoOperacao(unitOfWork).BuscarTiposOperacaoPorCNPJ(cnpj);
            });
        }

        /// <summary>
        /// Adicionar ou atualizar produtos.
        /// </summary>
        /// <param name="produto">Produto que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("SalvarProduto")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarProduto(produto, Auditado);
            });
        }

        /// <summary>
        /// Adicionar ou atualizar Carga.
        /// </summary>
        /// <param name="cargaIntegracao">Carga que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("AdicionarCarga")]
        public async Task<Retorno<Protocolos>> AdicionarCargaAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            return await ProcessarRequisicaoAsync((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AdicionarPedidoAsync(cargaIntegracao, false, cancellationToken);
            });
        }

        /// <summary>
        /// Atualizar Carga.
        /// </summary>
        /// <param name="Dados">Protocolo </param>
        /// <returns></returns>
        [HttpPost("AtualizarCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarCarga(ParametroParaAtualizarCarga Dados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarCargaPadrao(Dados.CargaIntegracao, Dados.Protocolo, unitOfWork.StringConexao, false);
            });
        }

        /// <summary>
        /// Solicitar Cancelamento Da Carga.
        /// </summary>
        /// <param name="solicitarCancelamentoCarga"></param>
        /// <returns></returns>
        [HttpPost("SolicitarCancelamentoDaCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDaCarga(SolicitacaoCancelamentoCarga solicitarCancelamentoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SolicitarCancelamentoDaCarga(solicitarCancelamentoCarga.protocoloIntegracaoCarga, solicitarCancelamentoCarga.motivoDoCancelamento, solicitarCancelamentoCarga.usuarioERPSolicitouCancelamento, solicitarCancelamentoCarga.ControleIntegracaoEmbarcador, false);
            });
        }

        /// <summary>
        /// Solicitar Cancelamento Do Pedido.
        /// </summary>
        /// <param name="solicitarCancelamentoPedido"></param>
        /// <returns></returns>
        [HttpPost("SolicitarCancelamentoDoPedido")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(SolicitarCancelamentoPedido solicitarCancelamentoPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SolicitarCancelamentoDoPedido(solicitarCancelamentoPedido.protocoloIntegracaoPedido, solicitarCancelamentoPedido.motivoDoCancelamento);
            });
        }

        /// <summary>
        /// Salvar Documento de Transporte
        /// </summary>
        /// <param name="documentoTransporte"></param>
        /// <returns></returns>
        [HttpPost("SalvarDocumentoTransporte")]
        public Dominio.ObjetosDeValor.WebService.Retorno<int> SalvarDocumentoTransporte(DocumentoTransporte documentoTransporte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarDocumentoTransporte(documentoTransporte, integradora);
            });
        }

        /// <summary>
        /// Agrupar Cargas
        /// </summary>
        /// <param name="protocoloCargas"></param>
        /// <returns></returns>
        [HttpPost("AgruparCargas")]
        public Dominio.ObjetosDeValor.WebService.Retorno<int> AgruparCargas(List<int> protocoloCargas)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AgruparCargas(protocoloCargas);
            });
        }

        /// <summary>
        /// Agrupar Cargas por Código Carga Embarcador
        /// </summary>
        /// <param name="agruparCargaPorCodigoCarga"></param>
        /// <returns></returns>
        [HttpPost("AgruparCargaPorCodigoCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<int> AgruparCargaPorCodigoCarga(AgruparCargaPorCodigoCarga agruparCargaPorCodigoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AgruparCargasPorCodigoCargaEmbarcador(agruparCargaPorCodigoCarga);
            });
        }

        /// <summary>
        /// Metodo para Buscar Cargas
        /// </summary>
        /// <param name="protocolo">Protocolos para uso de Buscar</param>
        /// <returns></returns>
        [HttpPost("BuscarCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCargaService(protocolo);
            });
        }

        /// <summary>
        /// Metodo para Buscar Cargas por datas
        /// </summary>
        /// <param name="dataDe">data indicando o inicio</param>
        /// <param name="dataAte">data indicando o fim</param>
        /// <returns></returns>
        [HttpPost("BuscarCargaPorDatas")]
        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaPorDatas(DateTime dataDe, DateTime dataAte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Periodo periodo = new Periodo { DataCarregamentoInicial = dataDe, DataCarregamentoFinal = dataAte };
                return new Servicos.WebService.Carga.Carga(unitOfWork).BuscarCargasPorPeriodo(periodo);
            });
        }

        /// <summary>
        /// Metodo para Buscar Cargas por período
        /// </summary>
        /// <param name="periodo">Períodos de tempo para buscar cargas</param>
        /// <returns></returns>
        [HttpPost("BuscarCargasPorPeriodo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargasPorPeriodo(Periodo periodo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork).BuscarCargasPorPeriodo(periodo);
            });
        }

        /// <summary>
        /// Metodo para Buscar o Transportador da Carga 
        /// </summary>
        /// <param name="protocolo">Protocolos para uso de uscar</param>
        /// <returns></returns>
        [HttpPost("BuscarTransportadorCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> BuscarTransportadorCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork).BuscarTransportadorCarga(protocolo);
            });
        }


        /// <summary>
        /// Metodo Para Recibimento de Carga Completa.
        /// </summary>
        /// <param name="cargaIntegracaoCompleta">Dados para a geração de uma nova carga Completa</param>
        /// <returns></returns>
        [HttpPost("AdicionarCargaCompleta")]
        public Dominio.ObjetosDeValor.WebService.Retorno<int> AdicionarCargaCompleta(CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AdicionarCargaCompleta(cargaIntegracaoCompleta);
            });
        }

        /// <summary>
        /// Metodo Para Confirmar Integração da Carga.
        /// </summary>
        /// <param name="protocolo">Protocolo das carga e pedido</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoCarga(Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).ConfirmarIntegracaoCarga(protocolo);
            });
        }

        /// <summary>
        /// Metodo Para Confirmar Integração programada da Carga.
        /// </summary>
        /// <param name="confirmarIntegracaoDeEnvioProgramado">dados para confirmar integração</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoDeEnvioProgramado")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoDeEnvioProgramado(ConfirmarIntegracaoDeEnvioProgramado confirmarIntegracaoDeEnvioProgramado)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).ConfirmarIntegracaoDeEnvioProgramado(confirmarIntegracaoDeEnvioProgramado);
            });
        }

        /// <summary>
        /// Metodo Para Buscar Carga Pendentes de Integração
        /// </summary>
        /// <param name="requestCargasIntegracaoPendentes">Paramentros para busca de cargas</param>
        /// <returns></returns>
        [HttpPost("BuscarCargasPendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasPendentesIntegracao(RequestCargasIntegracaoPendentes requestCargasIntegracaoPendentes)
        {
            return ProcessarRequisicaoAsync((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarCargasPendentesIntegracaoAsync(requestCargasIntegracaoPendentes, integradora);
            }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Metodo Para avançar etapa Frete
        /// </summary>
        /// <param name="protocolo">Paramentros para avançar carga</param>
        /// <returns></returns>
        [HttpPost("ConfirmarFrete")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarFrete(Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarFrete(protocolo);
            });
        }


        /// <summary>
        /// Metodo para Buscar Modelos Veiculares de Carga Pendentes de Integração
        /// </summary>
        /// <param name="quantidade">Indica quantidade de registros que deseja processar(Opcional). Por padrão é 50</param>
        /// <returns></returns>
        [HttpPost("BuscarModelosVeicularesPendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeicularesPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarModelosVeicularesPendentesIntegracao(quantidade);
            });
        }

        /// <summary>
        /// Confirma os Modelos Veiculares de Carga Pendentes Integrados
        /// </summary>
        /// <param name="protocolos">Lista De Protocolos dos Modelos Veiculares Integrados</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoModeloVeicular")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoModeloVeicular(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoModeloVeicular(protocolos);
            });
        }


        /// <summary>
        ///Informa os dados de transporta da carga
        /// </summary>
        /// <param name="dadosTransporte">Dados para vincular a carga</param>
        /// <returns></returns>
        [HttpPost("InformarDadosTransporteCarga")]

        public Retorno<bool> InformarDadosTransporteCarga(Dominio.ObjetosDeValor.WebService.Carga.DadosTransporte dadosTransporte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment), WebServiceConsultaCTe).InformarDadosTransporteCarga(dadosTransporte, integradora);
            });
        }

        /// <summary>
        /// Obtem Cargas Aguardando Envio Documentos
        /// </summary>
        /// <param name="requestPaginacao">Dados para vincular a carga</param>
        /// <returns></returns>
        [HttpPost("ObterCargasAguardandoEnvioDocumentos")]

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> ObterCargasAguardandoEnvioDocumentos(RequestPaginacao requestPaginacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterCargasAguardandoEnvioDocumentos(requestPaginacao, integradora);
            });
        }

        /// <summary>
        /// Confirma os Documentos integrados
        /// </summary>
        /// <param name="protocolos">Protocolos das cargas a integrar</param>
        /// <returns></returns>
        [HttpPost("ConfirmarRecebimentoCargaAguardandoEnvioDocumentos")]
        public Retorno<bool> ConfirmarRecebimentoCargaAguardandoEnvioDocumentos(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarRecebimentoCargaAguardandoEnvioDocumentos(protocolos);
            });
        }

        /// <summary>
        /// Retorna os pre documentos por cargas
        /// </summary>
        /// <param name="protocoloCarga">Protocolos da carga para pesquisa de dados</param>
        /// <returns></returns>
        [HttpPost("RetornarPreDocumentosPorCarga")]
        public Retorno<List<PreDocumento>> RetornarPreDocumentosPorCarga(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornarPreDocumentosPorCarga(protocoloCarga);
            });
        }

        /// <summary>
        /// Retorna o protocolo da carga para o número de carregamento informado.
        /// </summary>
        /// <param name="pedidoNumeroCarregamento">Número de carregamento do pedido</param>
        /// <returns></returns>
        [HttpPost("ConsultarCargaPorNumeroCarregamento")]
        public Retorno<int> ConsultarCargaPorNumeroCarregamento(string pedidoNumeroCarregamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConsultarCargaPorNumeroCarregamento(pedidoNumeroCarregamento);
            });
        }

        /// <summary>
        /// Retorna as cargas do transportador.
        /// </summary>
        /// <returns></returns>
        [HttpPost("BuscarCargasPorTransportador")]
        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargasPorTransportador()
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCargasPorTransportador(integradora.Empresa, null);
            });
        }
        /// <summary>
        /// Ajusta datas do pedido
        /// </summary>
        /// <param name="ajusteDatasPedido">Ajustes das datas</param>
        /// <returns></returns>
        [HttpPost("AjustarDatasPedido")]
        public Retorno<bool> AjustarDatasPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDatasPedido ajusteDatasPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AjustarDatasPedido(ajusteDatasPedido);
            });
        }

        /// Ajusta NumeroOrdem Pedido
        /// </summary>
        /// <param name="ajusteDatasPedido">Ajustes das datas</param>
        /// <returns></returns>
        [HttpPost("AjustarNumeroOrdemPedido")]
        public Retorno<bool> AjustarNumeroOrdemPedido(Dominio.ObjetosDeValor.WebService.Carga.AjustarNumeroOrdemPedido ajusteNumeroOrdemPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AjustarNumeroOrdemPedido(ajusteNumeroOrdemPedido);
            });
        }

        /// <summary>
        /// Solicitar Emissão dos Documentos.
        /// </summary>
        /// <param name="solicitarEmissaoDocumentos">Solicitar Emissão dos Documentos para ir para a Etapa de Emissão.</param>
        /// <returns></returns>
        [HttpPost("SolicitarEmissaoDocumentos")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SolicitarEmissaoDocumentos(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarFrete(new Protocolos() { protocoloIntegracaoCarga = protocoloCarga });
            });
        }

        /// <summary>
        /// Busca informações da carga apartir do totem .
        /// </summary>
        /// <param name="TotemFilaH">Dados do request </param>
        /// <returns></returns>
        [HttpPost("TotemFilaH")]
        public Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse TotemFilaH(FilaHRequest request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterDadosTotemFilaH(request);
            }).Objeto;
            //Autorizado pelo cesar
        }

        /// <summary>
        /// Retornar etapa de nota.
        /// </summary>
        /// <param name="protocoloCarga">Protocolo da carga</param>
        /// <returns></returns>
        [HttpPost("RetornarEtapaNota")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornarEtapaNota(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornarEtapaNota(protocoloCarga);
            });
        }

        /// <summary>
        /// Alterar número da carga.
        /// </summary>
        /// <param name="alterarNumeroCarga">Alterar número da carga.</param>
        /// <returns></returns>
        [HttpPost("AlterarNumeroCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AlterarNumeroCarga(AlterarNumeroCarga alterarNumeroCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AlterarNumeroCarga(alterarNumeroCarga);
            });
        }

        /// <summary>
        /// Busca o status (enviado pelo SAP) atual da DT/Carga.
        /// </summary>
        /// <param name="obterStatusDT">Obter status da DT.</param>
        /// <returns></returns>
        [HttpPost("ObterStatusDT")]
        public Dominio.ObjetosDeValor.WebService.Carga.StatusDTResponse ObterStatusDT(StatusDTRequest request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterStatusDT(request);
            }).Objeto;
        }

        /// <summary>
        /// Gerar Carregamento Roteirização.
        /// </summary>
        /// <param name="carregamentoRoteirizacao">Carregamento Roteirização</param>
        /// <returns></returns>
        [HttpPost("GerarCarregamentoRoteirizacao")]
        public Retorno<int> GerarCarregamentoRoteirizacao(Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao carregamentoRoteirizacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).GerarCarregamentoRoteirizacao(carregamentoRoteirizacao, integradora, true);
            });
        }

        /// <summary>
        /// Gerar Carregamento Roteirização em Lote.
        /// </summary>
        /// <param name="carregamentosRoteirizacao">Carregamento Roteirização em Lote</param>
        /// <returns></returns>
        [HttpPost("GerarCarregamentoRoteirizacaoEmLote")]
        public async Task<Retorno<RetornoAdicionarRequestAssincrono>> GerarCarregamentoRoteirizacaoEmLote(List<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao> carregamentosRoteirizacao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                // ⚡ Nova assinatura - apenas tipos de etapas
                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.QuebrarRequest,
                    TipoEtapaTarefa.GerarCarregamentoRoterizacao,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                RetornoAdicionarRequestAssincrono retornoAdicionarRequestAssincrono = await _adicionarRequestAssincrono.SalvarLoteAsync(carregamentosRoteirizacao, TipoRequest.GerarCarregamentoRoteirizacaoEmLote, etapas, cancellationToken, integradora.Codigo);

                return Retorno<RetornoAdicionarRequestAssincrono>.CriarRetornoSucesso(retornoAdicionarRequestAssincrono, "Lote adicionado para processamento.");
            });
        }

        /// <summary>
        /// Gerar Carregamento Roteirização.
        /// </summary>
        /// <param name="carregamentoRoteirizacao">Carregamento Roteirização</param>
        /// <returns></returns>
        [HttpPost("AtualizarCarregamentoRoteirizacao")]
        public async Task<Retorno<int>> AtualizarCarregamentoRoteirizacao(Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao carregamentoRoteirizacao)
        {
            return await ProcessarRequisicaoAsync((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarCarregamentoRoteirizacao(carregamentoRoteirizacao, cancellationToken, wsRest: true);
            });
        }

        /// <summary>
        /// Gerar Carregamento.
        /// </summary>
        /// <param name="carregamento">Gerar Carregamento.</param>
        /// <returns></returns>
        [HttpPost("GerarCarregamento")]
        public Retorno<int> GerarCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).GerarCarregamento(carregamento, true);
            });
        }

        /// <summary>
        /// Gerar Carregamento para processamento posterior.
        /// </summary>
        /// <param name="carregamento">Gerar Carregamento.</param>
        /// <returns></returns>
        [HttpPost("GerarCarregamentoAsync")]
        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> GerarCarregamentoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ValidarDadosParaGerarCarregamentoAsync(carregamento);

                // ⚡ Nova assinatura - apenas tipos de etapas
                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.GerarCarregamento,
                    TipoEtapaTarefa.FecharCarga,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                await _adicionarRequestAssincrono.SalvarAsync(carregamento, TipoRequest.GerarCarregamento, etapas, cancellationToken, integradora.Codigo);

                return Retorno<bool>.CriarRetornoSucesso(true, "Lote adicionado para processamento.");
            });
        }

        /// <summary>
        /// Gerar Carregamento.
        /// </summary>
        /// <param name="carregamento">Gerar Carregamento.</param>
        /// <returns></returns>
        [HttpPost("GerarCarregamentoNovoAsync")]
        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<int>> GerarCarregamentoNovoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).GerarCarregamentoNovoAsync(carregamento, cancellationToken, true, fecharCarga: true);
            });
        }

        /// <summary>
        /// Gerar Carregamentos com redespachos para processamento posterior.
        /// </summary>
        /// <param name="carregamento">Gerar Carregamento com Redespacho.</param>
        /// <returns></returns>
        [HttpPost("GerarCarregamentoComRedespachosAsync")]
        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> GerarCarregamentoComRedespachosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ValidarDadosParaGerarCarregamentoAsync(carregamento, validarProximoTrecho: true);

                // ⚡ Nova assinatura - apenas tipos de etapas
                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.QuebrarRequest,
                    TipoEtapaTarefa.GerarCarregamentoComRedespachos,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                await _adicionarRequestAssincrono.SalvarAsync(carregamento, TipoRequest.GerarCarregamentoComRedespachos, etapas, cancellationToken, integradora.Codigo);

                return Retorno<bool>.CriarRetornoSucesso(true);
            });
        }

        /// <summary>
        /// Obter Dados do Relatório de Paradas.
        /// </summary>
        /// <param name="dadosObterCargaPorDataCriacao">Obter dados do relatório de Paradas pela Data de Criação da Carga. O formato da data deve ser 0000-00-00T00:00:00.</param>
        /// /// <param name="NumeroPagina">Número da página para paginação. Valor padrão é 1.</param>
        /// <returns></returns>
        [HttpPost("ObterDadosParadasPorDataCriacaoCarga")]
        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>> ObterDadosParadasPorDataCriacaoCarga(DadosObterCargaPorDataCriacao dadosObterCargaPorDataCriacao, [FromQuery] int NumeroPagina = 1)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterDadosParadasPorDataCriacaoCarga(dadosObterCargaPorDataCriacao, NumeroPagina);
            });
        }

        /// <summary>
        /// Receber Pacotes via WebHook
        /// </summary>
        /// <param name="ReceberPacotesWebHook">Receber Pacotes via WebHook.</param>
        /// <returns></returns>
        [HttpPost("ReceberPacotesWebHook")]
        public Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook ReceberPacotesWebHook(Dominio.ObjetosDeValor.WebService.Carga.PacoteWebHook jwt)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment));
            Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook retorno = new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ReceberPacotesWebHook(jwt.jwt);
            if (retorno.retcode == 0)
                Response.StatusCode = 200;
            else
                Response.StatusCode = 422;
            return retorno;
        }

        /// <summary>
        /// Receber Pacotes via WebHook por fila 
        /// </summary>
        /// <param name="ReceberPacotesWebHookFila">Fila para receber Pacotes via WebHook.</param>
        /// <returns></returns>
        [HttpPost("ReceberPacotesWebHookFila")]

        public Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook ReceberPacotesWebHook2(Dominio.ObjetosDeValor.WebService.Carga.PacoteWebHook jwt)
        {
            try
            {
                string path = Utilidades.IO.FileStorageService.Storage.Combine(AppDomain.CurrentDomain.BaseDirectory, "FilaPacotes");
                string file = Utilidades.IO.FileStorageService.Storage.Combine(path, jwt.jwt);

                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.WriteLine(jwt.jwt);
                }

                Response.StatusCode = 200;
                return new Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook()
                {
                    message = $"SUCCESS",
                    retcode = 0
                };
            }
            catch (Exception e)
            {
                Response.StatusCode = 422;
                return new Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook()
                {
                    message = $"SERVER_ERROR {e.Message}",
                    retcode = -100002
                };
            }
        }

        /// <summary>
        /// Fechar a carga
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo de integração da carga</param>
        /// <returns></returns>
        [HttpPost("FecharCarga")]
        public Retorno<bool> FecharCarga(int protocoloIntegracaoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Carga.ProtocoloIntegracao integracao = new ProtocoloIntegracao();

                integracao.protocoloIntegracaoCarga = protocoloIntegracaoCarga;
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).FecharCarga(integracao);
            });
        }

        /// <summary>
        /// Fechar a carga
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo de integração da carga</param>
        /// <returns></returns>
        [HttpPost("FecharCargaIntegracao")]
        public Retorno<bool> FecharCargaIntegracao(Dominio.ObjetosDeValor.WebService.Carga.ProtocoloIntegracao protocoloIntegracaoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).FecharCarga(protocoloIntegracaoCarga);
            });
        }

        /// <summary>
        /// Atualização de Datas da Carga.
        /// </summary>
        /// <param name="ProtocoloCarga">Protocolo de integração da carga</param>
        /// <param name="NumeroCarga">Número da Carga</param>
        /// <param name="DataInicioCarregamento">Data de início do carregamento (Formato: "dd/MM/yyyy HH:mm:ss")</param>
        /// <param name="DataTerminoCarregamento">Data de termino do carregamento (Formato: "dd/MM/yyyy HH:mm:ss")</param>
        /// <param name="DataPrevisaoInicioViagem">Data de previsão de início de viagem (Formato: "dd/MM/yyyy HH:mm:ss")</param>
        /// <param name="DataLoger">Data Loger (Formato: "dd/MM/yyyy HH:mm:ss")</param>
        /// <param name="StatusLoger">Status Loger</param>
        /// <returns></returns>
        [HttpPost("AtualizarDatasCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarDatasCarga(Dominio.ObjetosDeValor.WebService.Rest.AtualizarDatasCarga atualizardatasCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarDatasCarga(atualizardatasCarga);
            });
        }

        /// <summary>
        /// Busca a situação da carga
        /// </summary>
        /// <param name="protocolo">Parâmetros para busca de cargas</param>
        /// <returns></returns>
        [HttpPost("BuscarSituacaoCarga")]
        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>> BuscarSituacaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarSituacaoCarga(protocolo);
            });
        }

        /// <summary>
        /// Solicita o cancelamento dos documentos da carga
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo de integração da carga</param>
        /// <param name="motivoDoCancelamento">Motivo do cancelamento</param>
        /// <param name="usuarioERPSolicitouCancelamento">Usuário ERP que solicitou o cancelamento</param>
        /// <returns></returns>
        [HttpPost("SolicitarCancelamentoDosDocumentosDaCarga")]
        public Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDosDocumentosDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SolicitarCancelamentoDosDocumentosDaCarga(protocoloIntegracaoCarga, motivoDoCancelamento, usuarioERPSolicitouCancelamento);
            });
        }

        /// <summary>
        /// Remove um pedido da carga
        /// </summary>
        /// <param name="protocolo">Parâmetros para busca de carga e pedido</param>
        /// <returns></returns>
        [HttpPost("RemoverPedido")]
        public Retorno<bool> RemoverPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RemoverPedido(protocolo);
            });
        }

        /// <summary>
        /// Receber AgrupamentoCarga Informacoes Transportes Carga - APISUL
        /// </summary>
        /// <param name="Request">Receber Json com informações para agrupar ou salvar dados tranporte carga WebHook.</param>
        /// <returns></returns>
        [HttpPost("AgruparCargasReceberDadosTransporte")]
        public Retorno<bool> AgruparCargasReceberDadosTransporte(Dominio.ObjetosDeValor.WebService.Rest.AgrupamentoCargasReceberDadosTransporte agrupamentoCargasReceberDadosTransporte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AgruparCargasReceberDadosTransporte(agrupamentoCargasReceberDadosTransporte);
            });
        }

        /// <summary>
        /// DesbloquearCargaBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo de integração da carga</param>
        /// <returns></returns>
        [HttpPost("DesbloquearCargaBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual")]
        public Retorno<bool> DesbloquearCargaBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual(int protocoloIntegracaoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Carga.ProtocoloIntegracao integracao = new ProtocoloIntegracao();

                integracao.protocoloIntegracaoCarga = protocoloIntegracaoCarga;
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).DesbloquearCargaBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual(integracao);
            });
        }

        /// <summary>
        /// ConsultarDetalhesCancelamentoDaCarga
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo de integração da carga</param>
        /// <returns></returns>
        [HttpPost("ConsultarDetalhesCancelamentoDaCarga")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> ConsultarDetalhesCancelamentoDaCarga(int protocoloIntegracaoCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConsultarDetalhesCancelamentoDaCarga(protocoloIntegracaoCarga);
            });
        }

        /// <summary>
        ///Busca carregamentos pendentes
        /// </summary>
        /// <param name="carregamentosPendentesIntegracao">Dados para buscar carregamentos pendentes</param>
        /// <returns></returns>
        [HttpPost("BuscarCarregamentosPendentesIntegracao")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracao(RequestCarregamentosPendentesIntegracao carregamentosPendentesIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment), WebServiceConsultaCTe).BuscarCarregamentosPendentesIntegracao(carregamentosPendentesIntegracao, integradora);
            });
        }

        /// <summary>
        /// Confirmar integracao carregamento
        /// </summary>
        /// <param name="protocolo">Protocolo de integração da carga</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoCarregamento")]
        public Retorno<bool> ConfirmarIntegracaoCarregamento(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoCarregamento(protocolo);
            });
        }

        /// <summary>
        /// Encerra a carga informada.
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo de integração da carga</param>
        /// <param name="observacaoEncerramento">Observação do encerramento da integração da carga</param>
        /// <returns></returns>
        [HttpPost("EncerrarCarga")]
        public Retorno<bool> EncerrarCarga(int protocoloIntegracaoCarga, string observacaoEncerramento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).EncerrarCarga(protocoloIntegracaoCarga, observacaoEncerramento);
            });
        }

        [HttpPost("ATS/InformacaoViagem")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformacaoViagem(Dominio.ObjetosDeValor.WebService.Carga.InformacaoViagemATS informacaoViagem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment), TipoSessaoBancoDados.Nova);

            try
            {
                Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);

                Dominio.Entidades.WebService.Integradora integradora = repositorioIntegradora.BuscarPorUsuarioESenha(informacaoViagem.Usuario, informacaoViagem.Senha);

                if (integradora == null || !(integradora?.Ativo ?? false))
                    throw new WebServiceException("Não autorizado. Token inválido.");

                return new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).IntegracaoEncerrarCarga(informacaoViagem?.DadosViagem);

            }
            catch (BaseException ex)
            {
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }

        }
        #endregion

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceCargas;
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
