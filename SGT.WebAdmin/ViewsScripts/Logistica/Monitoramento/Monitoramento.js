/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Global/ModeloFiltroPesquisa/ConfiguracaoModeloFiltroPesquisa.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatusViagemTipoRegra.js" />
/// <reference path="../../Enumeradores/EnumAlertaMonitorStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoFiltroCliente.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/FaixasTemperatura.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/TipoTrecho.js" />
/// <reference path="../Tracking/Tracking.lib.js" />
/// <reference path="./MonitoramentoSignalR.js" />
/// <reference path="Auditoria.js" />

var _cargaSelecionadaTrocaCompativel
var _gridMonitoramento;
var _gridAlertas;
var _pesquisaMonitoramento;
var _graficoAlertaResumo;
var _alertaResumo;
var _alertaDetalhe;
var _gridCargasCompativeis;
var _mapaMonitoramento;
var _CRUDTratativaAlerta;
var _gridResumoCarga;
var _chartAlertaResumo;
var _gridMapaParadas;
var _alteracaoDataPrevisoes;
var _CRUDMapaAdicionarPosicao;
var _CRUDAlterarFaixaTemperatura;
var _mapaNovaPosicao;
var _novaFaixaTemperatura;
var _newShape;
var _utilizarModalAntigoDetalhesMonitoramento = _CONFIGURACAO_TMS.UtilizarModalAntigoDetalhesMonitoramento;

//var tiposFiltroCliente = [
//    { text: "Nenhum", value: 0 },
//    { text: "Estão em alvo", value: this.Iniciado },
//    { text: "Possui entrega", value: this.Finalizado },
//    { text: "Cancelado", value: this.Cancelado }
//];

/*
 * Declaração das Classes
 */

var PesquisaMonitoramento = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaMonitoramento)) {
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
                    _pesquisaMonitoramento.ExibirFiltros.visibleFade(false);
                    _alertaResumo.ExibirFiltrosGrafico.visibleFade(false);
                }
                _pesquisaMonitoramento.DescricaoAlerta.val("");
                //_pesquisaMonitoramento.StatusViagem.val([]);
                recarregarDados();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Carga.getFieldDescription(), col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Carga });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Veiculos.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Logistica.Monitoramento.Veiculo });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Pedido.getFieldDescription(), col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Pedido });
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NotaFiscal.getFieldDescription(), col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.NotaFiscal });
    this.MonitoramentoStatus = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.StatusMonitoramento.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(EnumMonitoramentoStatus.Iniciado), options: EnumMonitoramentoStatus.obterOpcoes(), def: EnumMonitoramentoStatus.Iniciado, placeholder: Localization.Resources.Logistica.Monitoramento.StatusMonitoramento });
    this.GrupoPessoa = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.GrupoPessoa.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.FuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Vendedor.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusViagem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.GrupoTipoOperacao = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoDeOperacao.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.DescricaoAlerta = PropertyEntity();
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Transportador.getFieldDescription()), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCriacaoCarga, getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable(""), placeholder: Localization.Resources.Gerais.Geral.DataInicial });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCriacaoCarga, getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable(""), placeholder: Localization.Resources.Gerais.Geral.DataFinal });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    if (_CONFIGURACAO_TMS.TelaMonitoramentoPadraoFiltroDataInicialFinal) {
        this.DataInicial.val(Global.Data(EnumTipoOperacaoDate.Subtract, 2, EnumTipoOperacaoObjetoDate.Days));
        this.DataFinal.val(Global.Data(EnumTipoOperacaoDate.Add, 3, EnumTipoOperacaoObjetoDate.Days));
    }
    this.FiltroCliente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.FiltrarClientes.getFieldDescription(), val: ko.observable(EnumMonitoramentoFiltroCliente.Nenhum), options: EnumMonitoramentoFiltroCliente.obterOpcoes(), def: EnumMonitoramentoFiltroCliente.Nenhum });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Cliente.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.CategoriaPessoa = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Categoria.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.SomenteRastreados = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Gerais.Geral.SomenteVeiculosComRastreadorOnline, getType: typesKnockout.bool });
    this.SomenteUltimoPorCarga = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Gerais.Geral.SomenteUltimoPorCarga, getType: typesKnockout.bool });
    this.Filial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Gerais.Geral.Filial });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisaRelatorio(), def: EnumSituacaoCargaJanelaCarregamento.Todas, text: Localization.Resources.Gerais.Geral.SituacaoJanelaCarregamento.getFieldDescription() });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Gerais.Geral.NumeroEXP.getFieldDescription(), maxlength: 150, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntregaPedidoInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial });
    this.DataEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal });
    this.PrevisaoEntregaInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio });
    this.PrevisaoEntregaFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal });
    this.InicioViagemPrevistaInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialDaPrevisaoDeInicioDeViagem.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), def: "" });
    this.InicioViagemPrevistaFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalDaPrevisaoDeInicioDeViagem.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), def: "" });

    this.DataEmissaoNFeInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe });
    this.DataEmissaoNFeFim = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe });
    this.VeiculosComContratoDeFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.ApenasVeiculosQuePossuemContratoDeFrete, visible: ko.observable(true) });

    this.Origem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Origem.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Destino.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.Destino });
    this.EstadoOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoOrigem.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.EstadoOrigem });
    this.EstadoDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoDestino.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.EstadoDestino });
    this.NaoExibirResumosAlerta = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Logistica.Monitoramento.NaoExibirGraficoDeAlertas, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.CentroDeResultado.getFieldDescription(), idBtnSearch: guid() });
    this.ResponsavelVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.ResponsavelPeloVeiculo.getFieldDescription(), idBtnSearch: guid() });
    this.FronteiraRotaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.FronteiraRotaFrete.getFieldDescription(), idBtnSearch: guid() });
    this.PaisOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.PaisOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.PaisDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.PaisDestino.getFieldDescription(), idBtnSearch: guid() });
    this.ApenasMonitoramentosCriticos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.ApenasMonitoramentosCriticos, visible: ko.observable(true) });
    this.DataInicioCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCarregamento, getType: typesKnockout.dateTime, val: ko.observable(""), enable: ko.observable(true) });
    this.DataFimCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCarregamento, getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(true) });
    this.PossuiRecebedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiRecebedor.getFieldDescription(), val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "" });
    this.PossuiExpedidor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiExpedidor.getFieldDescription(), val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "" });
    this.Recebedores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Recebedor.getFieldDescription(), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Remetente.getFieldDescription(), idBtnSearch: guid() });
    this.CodigoCargaEmbarcadorMulti = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Logistica.Monitoramento.MultiplosNumerosCarga.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.TipoTrecho = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.TipoTrecho.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.VeiculosEmLocaisTracking = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.VeiculosEmLocais, visible: ko.observable(true) });
    this.LocaisTracking = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.LocaisTracking.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Logistica.Monitoramento.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Logistica.Monitoramento.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.Monitoramento,
        callbackRetornoPesquisa: function () {
            $("#" + _pesquisaMonitoramento.StatusViagem.id).trigger("change");
            $("#" + _pesquisaMonitoramento.GrupoTipoOperacao.id).trigger("change");
            $("#" + _pesquisaMonitoramento.FiltroCliente.id).trigger("change");
            $("#" + _pesquisaMonitoramento.MonitoramentoStatus.id).trigger("change");
        }
    });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.Monitoramento, _pesquisaMonitoramento) }, type: types.event, text: Localization.Resources.Gerais.Geral.ConfiguracaoDeFiltros, visible: ko.observable(true) });

    this.CodigoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Motorista.getFieldDescription(), idBtnSearch: guid() });
};

var AlertaResumo = function () {
    this.ExibirFiltrosGrafico = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltrosGrafico.visibleFade(!e.ExibirFiltrosGrafico.visibleFade());
        }, type: types.event, text: Localization.Resources.Logistica.Monitoramento.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ListaDadosAlertas = PropertyEntity({ val: ko.observableArray([]) });
};

var CRUDTratativaAlerta = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarTratativaClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarTratativaClick, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Tratativa = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Tratativa.getRequiredFieldDescription(), val: ko.observable(true), options: ko.observable([]), def: 1 });
    this.UtilizaTratativa = PropertyEntity({ val: ko.observable(true) });
    this.CodigoAlerta = PropertyEntity();
};

var CRUDMapaAdicionarPosicao = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarAdicionarNovaPosicaoClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarAdicionarNovaPosicaoClick, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
    this.LocalidadePosicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cidade.getFieldDescription(), idBtnSearch: guid() });
    this.Data = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Data.getRequiredFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.Latitude = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Latitude.getRequiredFieldDescription(), required: false, visible: ko.observable(true), maxlength: 20, enable: ko.observable(true) });
    this.Longitude = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Longitude.getRequiredFieldDescription(), required: false, visible: ko.observable(true), maxlength: 20, enable: ko.observable(true) });

    this.CodigoVeiculo = PropertyEntity();
    this.Codigo = PropertyEntity();
};

var CRUDAlterarFaixaTemperatura = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarAlterarFaixaTemperaturaClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarAlterarFaixaTemperaturaClick, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
    this.FaixaTemperatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Faixa Temperatura", idBtnSearch: guid() });

    this.CodigoMonitoramento = PropertyEntity();
    this.Codigo = PropertyEntity();
};

var PesquisaHistoricoPosicao = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaHistoricoPosicao))
                carregarDadosMapaHistoricoPosicao();
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
};

var AlertaDetalhe = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
    this.DataCadastro = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataCriacaoAlerta.getFieldDescription() });
    this.Data = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataEventoAlerta.getFieldDescription() });
    this.Tipo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoDoAlerta.getFieldDescription() });
    this.AlertaDescricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Valor.getFieldDescription() });
    this.Coordenadas = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Coordenadas.getFieldDescription() });
    this.Latitude = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Latitude.getFieldDescription() });
    this.Longitude = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Longitude.getFieldDescription() });
    this.Carga = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Carga.getFieldDescription() });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Veiculo.getFieldDescription() });
    this.Placa = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Veiculo.getFieldDescription() });
    this.Status = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Status.getFieldDescription() });
    this.StatusDescricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Status.getFieldDescription() });
    this.RegistroTratativa = PropertyEntity({ visible: ko.observable(true) });
    this.Tratativa = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Tratativa.getFieldDescription() });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Usuario.getFieldDescription() });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Data.getFieldDescription() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription() });
};

var AlteracaoNumeroRastreador = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
    this.NumeroRastreador = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NumeroRasterador.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 150 });

    this.Confirmar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Confirmar, type: types.event, val: ko.observable(false), eventClick: ConfirmarAlterarNumeroRasteradorClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, type: types.event, val: ko.observable(false), eventClick: CancelarAlteracaoNumeroRastreadorClick, visible: ko.observable(true) });
};

var AlteracaoDataPrevisoes = function () {
    this.CodigoProximaEntrega = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
    this.DataPrevisaoReprogramada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataPrevisaoReprogramada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataPrevisaoPlanejada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataPrevisaoPlanejada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Confirmar, type: types.event, val: ko.observable(false), eventClick: ConfirmarAlteracaoDataPrevisoesClick, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, type: types.event, val: ko.observable(false), eventClick: CancelaAlteracaoDataPrevisoesClick, visible: ko.observable(true) });
};


/*
 * Declaração das Funções de Inicialização
 */

function loadDroppable() {
    $("#container-grid-monitoramento").droppable({
        drop: itemSoltado,
        hoverClass: "ui-state-active backgroundDropHover",
    });
}

function loadMonitoramento() {
    loadPesquisaMonitoramento();

    SignalRPedidosMensagemChatEnviadaEvent = processarmsg;
    SignalRPedidosMensagemRecebidaEvent = processarmsg;

    buscarDetalhesOperador(function () {
        buscaStatusViagem(function () {
            loadGridMonitoramento();

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
                loadAlertaResumo();
                loadAlertaDetalhe();
                loadDroppable();
                loadAlteracaoNumeroRasterador();
                loadAlteracaoDataPrevisoes();
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
                _pesquisaMonitoramento.NaoExibirResumosAlerta.val(true);
                _pesquisaMonitoramento.NaoExibirResumosAlerta.visible(false);

                $("#knockoutAlertasResumo").hide();
            }

            loadDetalhesCarga();
            loadCRUDTratativaAlerta();
            loadCRUDAdicionarPosicao();
            loadCRUDAlterarFaixaTemperatura();
            loadDetalhesMonitoramento();
            loadHistoricoMonitoramento();
            AuditoriaMonitoramento();

            BuscarTransportadores(_pesquisaMonitoramento.Transportador, null, null, true);
            BuscarClientes(_pesquisaMonitoramento.Cliente);
            BuscarCategoriaPessoa(_pesquisaMonitoramento.CategoriaPessoa);
            BuscarGruposPessoas(_pesquisaMonitoramento.GrupoPessoa);
            BuscarFilial(_pesquisaMonitoramento.Filial);
            BuscarVeiculos(_pesquisaMonitoramento.Veiculo);
            BuscarFuncionario(_pesquisaMonitoramento.FuncionarioVendedor);
            BuscarTiposOperacao(_pesquisaMonitoramento.TipoOperacao);
            BuscarClientes(_pesquisaMonitoramento.Expedidor);
            BuscarLocalidades(_pesquisaMonitoramento.Origem);
            BuscarLocalidades(_pesquisaMonitoramento.Destino);
            BuscarEstados(_pesquisaMonitoramento.EstadoOrigem);
            BuscarEstados(_pesquisaMonitoramento.EstadoDestino);
            BuscarFuncionario(_pesquisaMonitoramento.ResponsavelVeiculo);
            BuscarCentroResultado(_pesquisaMonitoramento.CentroResultado);
            BuscarClientes(_pesquisaMonitoramento.FronteiraRotaFrete, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
            BuscarPaises(_pesquisaMonitoramento.PaisOrigem);
            BuscarPaises(_pesquisaMonitoramento.PaisDestino);
            BuscarLocais(_pesquisaMonitoramento.LocaisTracking, null, null, 4);
            BuscarCargas(_pesquisaMonitoramento.CodigoCargaEmbarcadorMulti);
            BuscarClientes(_pesquisaMonitoramento.Destinatario);
            BuscarClientes(_pesquisaMonitoramento.Remetente);
            BuscarClientes(_pesquisaMonitoramento.Recebedores);
            BuscarTiposTrecho(_pesquisaMonitoramento.TipoTrecho);
            BuscarMotoristas(_pesquisaMonitoramento.CodigoMotorista);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaMonitoramento.Filial.visible(false);
                _pesquisaMonitoramento.Transportador.visible(false);
                _pesquisaMonitoramento.GrupoPessoa.visible(true);
                _pesquisaMonitoramento.VeiculosComContratoDeFrete.visible(false);
                _pesquisaMonitoramento.NumeroEXP.visible(false);
            }

            var cssClass = "col col-xs-12 col-sm-2 col-md-2 col-lg-2";
            if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento) {
                buscaGrupoTipoOperacao(_pesquisaMonitoramento);
                _pesquisaMonitoramento.GrupoTipoOperacao.visible(true);
                if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
                    _pesquisaMonitoramento.StatusViagem.visible(false);
                } else {
                    _pesquisaMonitoramento.StatusViagem.visible(true);
                    cssClass = "col col-xs-12 col-sm-1 col-md-1 col-lg-1";
                }
            } else {
                _pesquisaMonitoramento.GrupoTipoOperacao.visible(false);
            }
            _pesquisaMonitoramento.DataInicial.cssClass(cssClass);
            _pesquisaMonitoramento.DataFinal.cssClass(cssClass);

            loadMonitoramentoControleEntrega(function () {
                registraComponente();
                loadEtapasControleEntrega();

                isMobile = $(window).width() <= 980;
                _containerControleEntrega = new ContainerControleEntrega();
                KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");
            });

            $("#" + _pesquisaMonitoramento.FiltroCliente.id).change(verificarPesquisaFiltroCliente);

            loadFiltroPesquisaMonitoramento();
            loadConexaoSignalRMonitoramento();
        }, _pesquisaMonitoramento.StatusViagem);
    });
}

function verificarPesquisaFiltroCliente() {
    var categoria = false, cliente = false;
    if (_pesquisaMonitoramento.FiltroCliente.val() != EnumMonitoramentoFiltroCliente.Nenhum) {
        categoria = true;
        cliente = true;
    }
    _pesquisaMonitoramento.CategoriaPessoa.enable(categoria);
    _pesquisaMonitoramento.Cliente.enable(cliente);
}

function loadMonitoramentoControleEntrega(callback) {
    carregarHTMLComponenteControleEntrega(callback);
}

function loadMapa() {
    if (!_mapaMonitoramento) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaMonitoramento = new MapaGoogle("map", false, opcoesmapa);
    }
}

function retornoLocalidade(data) {
    _CRUDMapaAdicionarPosicao.LocalidadePosicao.val(data.Cidade);
    _CRUDMapaAdicionarPosicao.LocalidadePosicao.codEntity(data.Codigo);

    _CRUDMapaAdicionarPosicao.Latitude.val(data.Latitude);
    _CRUDMapaAdicionarPosicao.Longitude.val(data.Longitude);

    var info = { latitude: parseFloat(String(data.Latitude).replace(',', '.')), longitude: parseFloat(String(data.Longitude).replace(',', '.')) };
    criarMarkerNovaPosicao(info);
}

function retornoFaixaTemperatura(data) {
    _CRUDAlterarFaixaTemperatura.FaixaTemperatura.val(data.Descricao);
    _CRUDAlterarFaixaTemperatura.FaixaTemperatura.codEntity(data.Codigo);
}

function loadMapaAdicionarPosicao() {
    var opcoes = new OpcoesMapa(false, false);

    if (_mapaNovaPosicao == null) {
        _mapaNovaPosicao = new MapaGoogle("mapaAdicionarPosicao", false, opcoes);
    }
}

function criarMarkerNovaPosicao(info) {
    if (_mapaNovaPosicao)
        _mapaNovaPosicao.clear();

    if (_newShape)
        google.maps.event.clearInstanceListeners(_newShape);

    if ((info.latitude) && (info.longitude) && (_mapaNovaPosicao)) {

        var marker = new ShapeMarker();

        marker.setPosition(info.latitude, info.longitude);

        marker.title = '<div>' + ' (' + info.latitude + ',' + info.longitude + ')' + '<div>';

        _mapaNovaPosicao.draw.setShapeDraggable(true);

        _newShape = _mapaNovaPosicao.draw.addShape(marker);
        _mapaNovaPosicao.direction.setZoom(17);
        _mapaNovaPosicao.direction.centralizar(info.latitude, info.longitude);

        _newShape.addListener("dragend", dragendEventEndereco)
    }
}

function dragendEventEndereco(event) {
    var latLng = event.latLng;
    //var latLng = _marker.getPosition();
    _CRUDMapaAdicionarPosicao.Latitude.val(latLng.lat().toFixed(6).toString());
    _CRUDMapaAdicionarPosicao.Longitude.val(latLng.lng().toFixed(6).toString());
}

function loadMapaHistoricoPosicao() {
    if (!_mapaHistoricoPosicao) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaHistoricoPosicao = new MapaGoogle("mapHistoricoPosicao", false, opcoesmapa);
    }
}

function loadCRUDTratativaAlerta() {
    _CRUDTratativaAlerta = new CRUDTratativaAlerta();
    KoBindings(_CRUDTratativaAlerta, "knockoutCRUDTratativaAlerta");
    limparCamposTratativa();
}

function loadPesquisaMonitoramento() {
    _pesquisaMonitoramento = new PesquisaMonitoramento();
    KoBindings(_pesquisaMonitoramento, "knockoutPesquisaMonitoramento", false, _pesquisaMonitoramento.Pesquisar.id);
}

function loadCRUDAdicionarPosicao() {
    _CRUDMapaAdicionarPosicao = new CRUDMapaAdicionarPosicao();
    KoBindings(_CRUDMapaAdicionarPosicao, "knockoutCRUDMapaAdicionarPosicao");

    BuscarLocalidades(_CRUDMapaAdicionarPosicao.LocalidadePosicao, null, null, retornoLocalidade);

    limparCamposAdicionarPosicao();
}

function loadCRUDAlterarFaixaTemperatura() {
    _CRUDAlterarFaixaTemperatura = new CRUDAlterarFaixaTemperatura();
    KoBindings(_CRUDAlterarFaixaTemperatura, "knockoutCRUDAlterarFaixasTemperatura");

    FaixasTemperatura(_CRUDAlterarFaixaTemperatura.FaixaTemperatura, null, null, retornoFaixaTemperatura);

    limparCamposAlterarFaixaTemperatura();
}

function loadAlertaResumo() {
    _alertaResumo = new AlertaResumo();
    KoBindings(_alertaResumo, "knockoutAlertasResumo");
    _alertaResumo.ListaDadosAlertas.val.removeAll();

    _chartAlertaResumo = new D3Funnel('#monitoramento-alerta-resumo-container');
    //loadGraficoAlertaResumo();
    //loadResumoCargas();
}

function loadResumoCargas() {
    _alertaResumo.ListaDadosAlertas.val.removeAll();

    executarReST(
        "Monitoramento/ObterDadosAlertaResumoCargasSituacao",
        RetornarObjetoPesquisa(_pesquisaMonitoramento),
        function (ret) {
            if (ret.Success) {
                if (ret.Data) {
                    if (ret.Data.length > 0) {
                        for (var i = 0; i < ret.Data.length; i++) {
                            _alertaResumo.ListaDadosAlertas.val.push(ret.Data[i]);
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, ret.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, ret.Msg);
            }
        });

    $("[rel=popover-hover]").popover({ trigger: "hover" });
}

function loadAlertaDetalhe() {
    _alertaDetalhe = new AlertaDetalhe();
    KoBindings(_alertaDetalhe, "knockoutAlertaDetalhe");
}

function loadGridMonitoramento() {
    var draggableRows = false;
    var limiteRegistros = 100;
    var totalRegistrosPorPagina = 100;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
        var opcaoDetalhesTorre = { descricao: "Detalhes Torre", id: guid(), evento: "onclick", metodo: visualizarDetalhesTorre, tamanho: "10", icone: "" };
        var opcaoDetalheMonitoramento = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDoMonitoramento, id: guid(), evento: "onclick", metodo: visualizarDetalhesMonitoramentoClick, tamanho: "10", icone: "" };
        var opcaoAlertas = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDosAlertas, id: guid(), evento: "onclick", metodo: visualizarAlertasClick, tamanho: "10", icone: "", visible: ko.observable() };
        var opcaoResumoCarga = { descricao: Localization.Resources.Logistica.Monitoramento.ResumoDasEntregas, id: guid(), evento: "onclick", metodo: visualizarResumoDaCargaClick, tamanho: "10", icone: "" };
        var opcaoDetalhesCarga = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDaCarga, id: guid(), evento: "onclick", metodo: visualizarDetalhesCargaClick, tamanho: "10", icone: "" };
        var opcaoDetalhesEntrega = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDaEntrega, id: guid(), evento: "onclick", metodo: visualizarDetalhesEntregaClick, tamanho: "10", icone: "" };
        var opcaoVisualizarMapa = { descricao: Localization.Resources.Logistica.Monitoramento.VisualizarNoMapa, id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
        var opcaoHistoricos = { descricao: Localization.Resources.Logistica.Monitoramento.Historicos, id: guid(), evento: "onclick", metodo: visualizarHistoricosClick, tamanho: "8", icone: "" };
        var opcaoAlterarMonitoramentoCarga = { descricao: Localization.Resources.Logistica.Monitoramento.BuscarHistoricoMonitoramentoCargaCanceladaCcompativel, id: guid(), evento: "onclick", metodo: visualizarCargasCanceladasCompativeis, tamanho: "10", icone: "" };
        var opcaoAlterarNumeroRastreador = { descricao: Localization.Resources.Logistica.Monitoramento.AlterarNumeroDoRastreador, id: guid(), evento: "onclick", metodo: visualizarAlteracaoNumeroRastreador, tamanho: "10", icone: "", visibilidade: visualizarOpcaoAlterarNumeroRastreador };
        var opcaoAlterarDataPrevisoes = { descricao: Localization.Resources.Logistica.Monitoramento.AlterarDatasPrevisoes, id: guid(), evento: "onclick", metodo: visualizarAlteracaoDataPrevisoes, tamanho: "10", icone: "" };
        var opcaoAdicionarPosicaoManual = { descricao: Localization.Resources.Logistica.Monitoramento.AdicionarPosicaoVeiculo, id: guid(), evento: "onclick", metodo: visualizarAdicionarPosicaoManualmente, tamanho: "10", icone: "", visibilidade: visualizarOpcaoMonitoramentoOff };
        var opcaoAlterarFaixaTemperatura = { descricao: "Alterar faixa de temperatura", id: guid(), evento: "onclick", metodo: visualizarAlteracaoFaixaTemperatura, tamanho: "10", icone: "" };
        var opcaoAuditarMonitoramento = { descricao: "Auditar monitoramento", id: guid(), evento: "onclick", metodo: exibirAuditoriaMonitoramentoClick, tamanho: "10", icone: "" };

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoDetalhesTorre, opcaoDetalheMonitoramento, opcaoAlertas, opcaoDetalhesCarga, opcaoResumoCarga, opcaoDetalhesEntrega, opcaoVisualizarMapa, opcaoHistoricos, opcaoAlterarMonitoramentoCarga, opcaoAlterarNumeroRastreador, opcaoAlterarDataPrevisoes, opcaoAdicionarPosicaoManual, opcaoAlterarFaixaTemperatura, opcaoAuditarMonitoramento], tamanho: 5, };
        var configuracoesExportacao = { url: "Monitoramento/ExportarPesquisa", titulo: Localization.Resources.Logistica.Monitoramento.Monitoramentos };

        _gridMonitoramento = new GridView("grid-monitoramento", "Monitoramento/Pesquisa", _pesquisaMonitoramento, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao, undefined, undefined, gridMonitoramentoCallbackRow, gridMonitoramentoCallbackColumnDefault);
    } else {
        // portal cliente
        var opcaoVisualizarMapa = { descricao: Localization.Resources.Logistica.Monitoramento.VisualizarNoMapa, id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoVisualizarMapa], tamanho: 5, };
        var configuracoesExportacao = { url: "Monitoramento/ExportarPesquisa", titulo: Localization.Resources.Logistica.Monitoramento.Monitoramentos };

        _gridMonitoramento = new GridView("grid-monitoramento", "Monitoramento/Pesquisa", _pesquisaMonitoramento, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao, undefined, undefined, gridMonitoramentoCallbackRow, gridMonitoramentoCallbackColumnDefault);
    }

    _gridMonitoramento.SetPermitirEdicaoColunas(true);
    _gridMonitoramento.SetSalvarPreferenciasGrid(true);
    _gridMonitoramento.SetHabilitarModelosGrid(true);
    _gridMonitoramento.SetHabilitarScrollHorizontal(true, 200);
}

function loadGridAlertas(carga, descricaoAlerta) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    var opcaoTratativaAlerta = { descricao: Localization.Resources.Logistica.Monitoramento.TratativaAlerta, id: guid(), evento: "onclick", metodo: TratativaAlertaClick, tamanho: "10", icone: "", visibilidade: visualizarAlertaTratativaVisibilidade };
    var opcaoVisualizarMapa = { descricao: Localization.Resources.Logistica.Monitoramento.VisualizarNoMapa, id: guid(), evento: "onclick", metodo: visualizarAlertaMapaClick, tamanho: "10", icone: "" };
    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), evento: "onclick", metodo: visualizarAlertaDetalhesClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoVisualizarMapa, opcaoTratativaAlerta, opcaoDetalhes], tamanho: 10, };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
        _gridAlertas = new GridView("grid-alertas", "Monitoramento/ObterAlertas?carga=" + carga + "&DescricaoAlerta=" + descricaoAlerta, null, menuOpcoes, null, totalRegistrosPorPagina, null, false, draggableRows, null,
            limiteRegistros, undefined, undefined, undefined, undefined, null);
    } else {
        _gridAlertas = new GridView("grid-alertas", "Monitoramento/ObterAlertas?carga=" + carga + "&DescricaoAlerta=" + descricaoAlerta, null, null, null, totalRegistrosPorPagina, null, false, draggableRows, null,
            limiteRegistros, undefined, undefined, undefined, undefined, null);
    }


    _gridAlertas.CarregarGrid();
}

function loadGridResumoCarga(carga) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    _gridResumoCarga = new GridView("grid-resumo-carga", "Monitoramento/ObterResumoCarga?carga=" + carga, null, null, { column: 0, dir: orderDir.asc }, totalRegistrosPorPagina, null, false, draggableRows, null,
        limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridResumoCarga.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function ExibirModalMapa() {
    $(".legenda-rotas-container").hide();
    Global.abrirModal("divModalMapa");
    $("#divModalMapa").one('hidden.bs.modal', function () {
        _mapaMonitoramento.direction.limparMapa();
    });
}

function ExibirModalAlertas() {
    Global.abrirModal("divModalAlerta");
}

function ExibirModalResumoCarga() {
    Global.abrirModal("divModalResumoCarga");
}

function ExibirModalTratativaAlerta() {
    Global.abrirModal("divModalTratativaAlerta");
}

function ExibirModalAdicionarPosicao() {
    Global.abrirModal("divModalPosicaoManual");
}

function ExibirModalAlterarFaixaTemperatura() {
    Global.abrirModal("divModalFaixasTemperatura");
}


function visualizarAlertasClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ExibirModalAlertas()
    loadGridAlertas(filaSelecionada.Carga, _pesquisaMonitoramento.DescricaoAlerta.val());
}

function visualizarResumoDaCargaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ExibirModalResumoCarga();
    loadGridResumoCarga(filaSelecionada.Carga);
}

function carregarDadosMapa(filaselecionada) {
    _mapaMonitoramento.clear();
    executarReST("Monitoramento/ObterDadosMapa", {
        Codigo: filaselecionada.Codigo,
        Carga: filaselecionada.Carga,
        Veiculo: filaselecionada.Veiculo,
        IDEquipamento: filaselecionada.IDEquipamento
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {
                    TrackingDesenharInformacoesMapa(_mapaMonitoramento, arg.Data);
                    TrackingCriarMarkerVeiculo(_mapaMonitoramento, arg.Data.Veiculo, false, 0)
                }, 1000);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function visualizarHistoricoPosicaoMapaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    var dataInicial = Global.DataHora(EnumTipoOperacaoDate.Subtract, 2, EnumTipoOperacaoObjetoDate.Hours);
    _pesquisaHistoricoPosicao.DataInicial.val(dataInicial);
    _pesquisaHistoricoPosicao.DataFinal.val(Global.DataHoraAtual());
    ExibirModalMapaHistoricoPosicao();
    loadMapaHistoricoPosicao();
    carregarDadosMapaHistoricoPosicao();
}

function visualizarHistoricosClick(filaSelecionada) {
    exibirHistoricoMonitoramentoPorCodigo(filaSelecionada.Codigo);
}

function visualizarMapaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ExibirModalMapa();
    loadMapa();
    carregarDadosMapa(filaSelecionada);

    var configuracoesExportacao = { url: "Monitoramento/ExportarParadas?codigo=" + filaSelecionada.Codigo, titulo: "ParadasCarga" };
    _gridMapaParadas = new GridView("grid-mapa-paradas", "Monitoramento/ObterParadas?codigo=" + filaSelecionada.Codigo, null, null, null, 10, null, true, null, null, null, true, configuracoesExportacao, null, true, null, false);
    _gridMapaParadas.CarregarGrid();

}

function carregarDadosAlertaMapa(data) {
    _mapaMonitoramento.clear();
    TrackingCriarMarkerVeiculo(_mapaMonitoramento, data, true, 1);
}

function visualizarAlertaMapaClick(filaSelecionada) {
    var data = {
        PlacaVeiculo: filaSelecionada.Placa,
        Latitude: filaSelecionada.Latitude,
        Longitude: filaSelecionada.Longitude,
        Descricao: filaSelecionada.Tipo + '<br/>' + filaSelecionada.Valor + '<br/>' + filaSelecionada.Data
    };

    ExibirModalMapa()
    loadMapa();
    carregarDadosAlertaMapa(data);
}

function visualizarAlertaDetalhesClick(row) {
    executarReST("Monitoramento/ObterDetalhesAlerta", {
        Codigo: row.Codigo
    }, function (retorno) {
        if (retorno.Success) {
            Global.abrirModal("divModalAlertaDetalhe");
            PreencherObjetoKnout(_alertaDetalhe, retorno);
            _alertaDetalhe.RegistroTratativa.visible(retorno.Data.Status == EnumAlertaMonitorStatus.Finalizado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function visualizarAlertaTratativaVisibilidade(row) {
    return (row.Status == EnumAlertaMonitorStatus.EmAberto || row.Status == EnumAlertaMonitorStatus.EmTratativa);
}

function visualizarDetalhesMonitoramentoClick(filaSelecionada) {
    exibirDetalhesMonitoramentoPorCodigo(filaSelecionada.Codigo);
}

function limparCamposTratativa() {
    LimparCampos(_CRUDTratativaAlerta);
    _CRUDTratativaAlerta.Tratativa.options([]);
}

function limparCamposAdicionarPosicao() {
    LimparCampos(_CRUDMapaAdicionarPosicao);
}

function limparCamposAlterarFaixaTemperatura() {
    LimparCampos(_CRUDAlterarFaixaTemperatura);
}

function confirmarAdicionarNovaPosicaoClick(e, sender) {
    Salvar(_CRUDMapaAdicionarPosicao, "Monitoramento/AdicionarPosicaoManualmente", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                Global.fecharModal("divModalPosicaoManual");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}


function cancelarAdicionarNovaPosicaoClick() {
    Global.fecharModal("divModalPosicaoManual");
}

function confirmarAlterarFaixaTemperaturaClick(e, sender) {
    Salvar(_CRUDAlterarFaixaTemperatura, "Monitoramento/AlterarFaixaTemperatura", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                Global.fecharModal("divModalFaixasTemperatura");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}


function cancelarAlterarFaixaTemperaturaClick() {
    Global.fecharModal("divModalFaixasTemperatura");
}

function cancelarTratativaClick() {
    Global.fecharModal("divModalAlertaDetalhe");
    limparCamposTratativa();
}

function confirmarTratativaClick(e, sender) {
    _CRUDTratativaAlerta.UtilizaTratativa.val(true);
    Salvar(_CRUDTratativaAlerta, "AlertaTratativa/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                Global.fecharModal("divModalTratativaAlerta");
                limparCamposTratativa();

                if (_gridAlertas) {
                    _gridAlertas.CarregarGrid();

                } else if (_gridMonitoramentoNovo) {
                    recarregarDadosMonitoramentoNovo();
                }

            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function visualizarDetalhesEntregaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    var carga = filaSelecionada.Carga;

    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.Monitoramento.NaoExisteCargaParaEsteVeiculo);
        return;
    }

    loadConfiguracaoWidget();

    executarReST("/ControleEntrega/ObterControleEntregaPorcarga", { Carga: carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.abrirModal("knoutContainerControleEntrega");

                _containerControleEntrega.Entregas.val([arg.Data.Entregas]);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

function loadDetalhesCarga(callback) {
    carregarConteudosHTML(function () {

        if (callback)
            callback();
    });
}

function visualizarDetalhesCargaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ObterDetalhesCargaFluxo(filaSelecionada.Carga);
}

function ObterDetalhesCargaFluxo(carga) {
    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.Monitoramento.NaoExisteCargaParaEsteVeiculo);
        return;
    }

    var data = { Carga: carga };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function loadAlteracaoNumeroRasterador() {
    _alteracaoNumeroRastreador = new AlteracaoNumeroRastreador();
    KoBindings(_alteracaoNumeroRastreador, "knockoutAlteracaoNumeroRastreador");
}

function loadAlteracaoDataPrevisoes() {
    _alteracaoDataPrevisoes = new AlteracaoDataPrevisoes();
    KoBindings(_alteracaoDataPrevisoes, "knockoutAlteracaoDataPrevisoes");
}


function visualizarAlteracaoNumeroRastreador(filaSelecionada) {
    LimparCampos(_alteracaoNumeroRastreador);
    atualizaTituloModalCarga(filaSelecionada);
    _alteracaoNumeroRastreador.Codigo.val(filaSelecionada.Veiculo);
    _alteracaoNumeroRastreador.NumeroRastreador.val(filaSelecionada.NumeroRastreador);
    ExibirModalAlteracaoNumeroRastreador();
}

function visualizarAlteracaoDataPrevisoes(filaSelecionada) {
    LimparCampos(_alteracaoDataPrevisoes);
    atualizaTituloModalCarga(filaSelecionada);

    _alteracaoDataPrevisoes.DataPrevisaoReprogramada.visible(false);
    _alteracaoDataPrevisoes.CodigoProximaEntrega.val(filaSelecionada.CodigoProximaEntrega);
    _alteracaoDataPrevisoes.DataPrevisaoPlanejada.val(filaSelecionada.DataEntregaPlanejadaProximaEntrega);

    if (filaSelecionada.RastreadorOnlineOffline == 1 || filaSelecionada.RastreadorOnlineOffline == 3) {
        _alteracaoDataPrevisoes.DataPrevisaoReprogramada.val(filaSelecionada.DataEntregaReprogramadaProximaEntrega);
        _alteracaoDataPrevisoes.DataPrevisaoReprogramada.visible(true);
    }

    if (parseInt(_alteracaoDataPrevisoes.CodigoProximaEntrega.val()) === 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.Monitoramento.CargaNaoPossuiProximaEntregaParaAlterarAsDatas);
        return;
    }

    ExibirModalAlteracaoDataPrevisoes();
}

function visualizarAdicionarPosicaoManualmente(filaSelecionada) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.Pedido.AcaoNaoEPermitida);
        return;
    }

    atualizaTituloModalCarga(filaSelecionada);
    _CRUDMapaAdicionarPosicao.CodigoVeiculo.val(filaSelecionada.Veiculo);
    _CRUDMapaAdicionarPosicao.Codigo.val(filaSelecionada.Codigo);

    loadMapaAdicionarPosicao();
    ExibirModalAdicionarPosicao();
}

function visualizarAlteracaoFaixaTemperatura(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    _CRUDAlterarFaixaTemperatura.CodigoMonitoramento.val(filaSelecionada.Codigo);
    _CRUDAlterarFaixaTemperatura.Codigo.val(filaSelecionada.Codigo);

    ExibirModalAlterarFaixaTemperatura();
}

function loadCargasCanceladasCompativeis(carga) {
    var draggableRows = false;
    var limiteRegistros = 60;
    var totalRegistrosPorPagina = 20;

    var opcaoSelecionar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: selecionarCargaCanceladaCompativel, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoSelecionar]
    };

    _gridCargasCompativeis = new GridView("grid-cargas-compativeis", "Monitoramento/ObterCargasCanceladasCompativeis?carga=" + carga, null, menuOpcoes, null, totalRegistrosPorPagina, null, false, draggableRows, null,
        limiteRegistros, undefined, undefined, undefined, undefined, null);

    _gridCargasCompativeis.CarregarGrid();
}

function visualizarCargasCanceladasCompativeis(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ExibirModalCargasCanceladasCompativeis();
    loadCargasCanceladasCompativeis(filaSelecionada.Carga);

    _cargaSelecionadaTrocaCompativel = filaSelecionada.Carga;
}

function selecionarCargaCanceladaCompativel(e) {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.Monitoramento.VoceRealmenteDesejaCarregarMonitoramentoCargaCancelada, function () {
        executarReST(
            "Monitoramento/TrocarMonitoramentoCargaCancelada",
            {
                CargaCancelada: e.Codigo.val(),
                CargaDestino: _cargaSelecionadaTrocaCompativel
            },
            function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                }
            }
        );
    });

}

function ExibirModalCargasCanceladasCompativeis() {
    Global.abrirModal("divModalCargasCanceladasCompativeis");
}

function ExibirModalAlteracaoNumeroRastreador() {
    Global.abrirModal("divModalAlteracaoNumeroRastreador");
}

function ExibirModalAlteracaoDataPrevisoes() {
    Global.abrirModal("divModalAlteracaoDataPrevisoes");
}

function ExibirModalAlteracaoFaixaTemperatura() {
    Global.abrirModal("divModalAlteracaoFaixaTemperatura");
}

function ConfirmarAlterarNumeroRasteradorClick() {
    if (_alteracaoNumeroRastreador.NumeroRastreador.val() == "" || _alteracaoNumeroRastreador.NumeroRastreador.val() == undefined || _alteracaoNumeroRastreador.NumeroRastreador.val() == null) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.Monitoramento.NecessarioInformarNumeroDoRastreador);
        return;
    }

    var data = {
        CodigoVeiculo: _alteracaoNumeroRastreador.Codigo.val(),
        NumeroRastreador: _alteracaoNumeroRastreador.NumeroRastreador.val()
    };

    executarReST("Monitoramento/AlterarNumeroRastreador", data, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
            CancelarAlteracaoNumeroRastreadorClick();
            _gridMonitoramento.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function CancelarAlteracaoNumeroRastreadorClick() {
    LimparCampos(_alteracaoNumeroRastreador);
    Global.fecharModal("divModalAlteracaoNumeroRastreador");
}

function ConfirmarAlteracaoDataPrevisoesClick() {

    var data = {
        CodigoProximaEntrega: _alteracaoDataPrevisoes.CodigoProximaEntrega.val(),
        DataPrevisaoReprogramada: _alteracaoDataPrevisoes.DataPrevisaoReprogramada.val(),
        DataPrevisaoPlanejada: _alteracaoDataPrevisoes.DataPrevisaoPlanejada.val()
    };
    executarReST("Monitoramento/AlterarDatasPrevisoes", data, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
            CancelaAlteracaoDataPrevisoesClick();

            if (_gridMonitoramento)
                _gridMonitoramento.CarregarGrid();
            else if (_gridMonitoramentoNovo)
                _gridMonitoramentoNovo.CarregarGrid();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}


function CancelaAlteracaoDataPrevisoesClick() {
    LimparCampos(_alteracaoDataPrevisoes);
    Global.fecharModal("divModalAlteracaoDataPrevisoes");
}


function visualizarOpcaoAlterarNumeroRastreador(filaSelecionada) {
    return (filaSelecionada.TipoIntegracaoTecnologiaRastreador != undefined && filaSelecionada.TipoIntegracaoTecnologiaRastreador != null && filaSelecionada.TipoIntegracaoTecnologiaRastreador == EnumTipoIntegracao.MixTelematics);
}

function visualizarOpcaoMonitoramentoOff(filaSelecionada) {
    return (filaSelecionada.RastreadorOnlineOffline == 1 || filaSelecionada.RastreadorOnlineOffline == 4) && filaSelecionada.Status != EnumMonitoramentoStatus.Finalizado && filaSelecionada.Status != EnumMonitoramentoStatus.Cancelado;
}

/*
 * Declaração das Funções
 */
function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Logistica.Monitoramento.PorFavorInformeOsCamposObrigatorios);
}

function itemSoltado(event, ui) {
    var idContainerDestino = event.target.id;
    var idContainerOrigem = "container-" + $(ui.draggable[0]).parent().parent()[0].id;
}

function recarregarDados() {
    _gridMonitoramento.CarregarGrid();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
        loadGraficoAlertaResumo();
        loadResumoCargas();
    }
}

function TratativaAlertaClick(filaSelecionada) {
    loadTratativaAlerta({ CodigoAlerta: filaSelecionada.Codigo }, []);
}

function tratativaAdicionarPosicaoVeiculoClick(filaSelecionada) {
    _CRUDMapaAdicionarPosicao.CodigoVeiculo.val(filaSelecionada.Tracao);
    ExibirModalAdicionarPosicao();
}

function graficoMonitoramentoClick(d) {
    _pesquisaMonitoramento.DescricaoAlerta.val(d.label.raw);
    _gridMonitoramento.CarregarGrid();
}

function resumoMonitoramentoClick(d) {
    var statusviagem = new Array();
    statusviagem.push(d.codigoStatus);

    _pesquisaMonitoramento.StatusViagem.val(statusviagem);
    loadGraficoAlertaResumo();
    loadResumoCargas();

    _gridMonitoramento.CarregarGrid();
}

function loadGraficoAlertaResumo() {
    var isIE = /*@cc_on!@*/false || !!document.documentMode;

    if (!_pesquisaMonitoramento.NaoExibirResumosAlerta.val()) {
        _alertaResumo.ExibirFiltrosGrafico.visibleFade(true);
        var options = {
            block: !isIE ? {
                dynamicHeight: false,
                minHeight: 15,
                highlight: true
            } : {},
            tooltip: {
                enabled: false,
            },
            chart: {
                //animate: 10,
                bottomPinch: 1,
                curve: {
                    enabled: true
                }
            },
            label: {
                fontFamily: '"Reem Kufi", sans-serif',
                fontSize: '16px'
            },
            events: {
                click: {
                    block: graficoMonitoramentoClick
                }
            }
        };

        executarReST(
            "Monitoramento/ObterDadosAlertaResumoVeiculo",
            RetornarObjetoPesquisa(_pesquisaMonitoramento),
            function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        if (arg.Data.length > 0) {
                            _chartAlertaResumo.draw(arg.Data, options);
                        } else {
                            _chartAlertaResumo.destroy();
                        }
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
    } else
        _alertaResumo.ExibirFiltrosGrafico.visibleFade(false);
}

function atualizaTituloModalCarga(row) {
    $(".title-carga-codigo-embarcador").html(row.CargaEmbarcador);
    $(".title-carga-placa").html(row.Tracao + " " + row.Reboques);
}

//function atualizacaoGraficoAlertaAutomatica() {
//    if ((document.URL) && (document.URL.toLowerCase().includes("logistica/monitoramento"))) {
//        loadGraficoAlertaResumo(true);
//        loadResumoCargas(true);
//    }
//}

function buscaStatusViagem(callback, statusViagem) {

    executarReST("MonitoramentoStatusViagem/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var selected = [];
                for (var i = 0; i < arg.Data.StatusViagem.length; i++) {
                    if (arg.Data.StatusViagem[i].selected == 'selected') {
                        selected.push(arg.Data.StatusViagem[i].value);
                    }
                }
                statusViagem.options(arg.Data.StatusViagem);
                statusViagem.val(selected);

                $("#" + statusViagem.id).selectpicker('refresh');

                callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function buscaGrupoTipoOperacao(_pesquisa) {
    executarReST("GrupoTipoOperacao/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _pesquisa.GrupoTipoOperacao.options(arg.Data.GrupoTipoOperacao);
                $("#" + _pesquisa.GrupoTipoOperacao.id).selectpicker('refresh');

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ParametrosPesquisaMonitoramento() {
    return {

    };
}

function statusMonitoramentoEncerrado(status) {
    return (status == EnumMonitoramentoStatus.Finalizado || status == EnumMonitoramentoStatus.Cancelado);
}

function loadFiltroPesquisaMonitoramento() {
    var data = { TipoFiltro: EnumCodigoFiltroPesquisa.Monitoramento };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisa(_pesquisaMonitoramento, res.Data.Dados);
            _pesquisaMonitoramento.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
            _pesquisaMonitoramento.ModeloFiltrosPesquisa.val(res.Data.Descricao);

            if (_pesquisaMonitoramento.ModeloFiltrosPesquisa.callbackRetornoPesquisa instanceof Function)
                _pesquisaMonitoramento.ModeloFiltrosPesquisa.callbackRetornoPesquisa();

            //_gridMonitoramento.CarregarGrid();
        }
    });
}

function processarmsg() {
    //
}