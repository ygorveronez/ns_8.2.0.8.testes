/// <reference path="../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Enumeradores/EnumConsultaPorEntregaStatus.js" />
/// <reference path="../../Enumeradores/EnumStatusViagemControleEntrega.js" />
/// <reference path="../../Enumeradores/EnumStatusAcompanhamento.js" />
/// <reference path="../../Enumeradores/EnumPreTrip.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Logistica/Tracking/Tracking.lib.js" />
/// <reference path="tratamentoalerta.js" />
/// <reference path="PedidosOutrasCargas.js" />
/// <reference path="signalr.js" />
/// <reference path="DetalhesCargaEspelhada.js" />
/// <reference path="../../Global/ModeloFiltroPesquisa/ConfiguracaoModeloFiltroPesquisa.js" />
/// <reference path="../../Enumeradores/EnumAlertaMonitorStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cardAtivo;
var _pesquisaAcompanhamentoCarga;
var _cardAcompanhamentoCarga;
var _cabecalhoAcompanhamentoCarga;
var _detalhesEntregas;
let _modalAnotacoesCarga;
var _detalhesCarga;
var _posicaoPin = 0;
var _mostrandoCards;
var _gridParadas;
var _raioXCarga;
var _cardCargaAlertaTratado;
var _alterarMotoristaAcompanhamentoCarga;
var _gridAlterarMotoristaAcompanhamentoCarga;
var _gridIntegracaoAlterarMotoristaAcompanhamentoCarga;
var _gridHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga;

var _alterarVeiculoAcompanhamentoCarga;
var _gridAlterarVeiculoAcompanhamentoCarga;
var _gridAlterarVeiculoAlterarMotoristaAcompanhamentoCarga;
var _gridIntegracaoAlterarVeiculoAcompanhamentoCarga;
var _gridHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga;

var _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.Todas;
var _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
var _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Todos;
var opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumSituacoesCarga.obterOpcoesEmbarcador() : EnumSituacoesCarga.obterOpcoesTMS();
var _InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = false;
var _FiltroEspelhamentoSelecionado = null;

$(window).scroll(function () {
    var scroll = $(window).scrollTop();

    if (scroll >= 20) {
        $(".topBar").css("top", "0");
    } else {
        $(".topBar").css("top", "50px");
    }
});

var PesquisaAcompanhamentoCarga = function () {
    this.NumeroCarga = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Cargas.ControleEntrega.NumeroCarga.getFieldDescription() });
    this.NumeroOrdem = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Cargas.ControleEntrega.NumeroOrdem.getFieldDescription() });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Pedido, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicioViagemInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioViagemRealizada.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicioViagemFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date });
    this.NumerosNotasFiscais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais, idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusViagemControleEntrega = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.SituacaoControleDeEntregas, val: ko.observable(EnumStatusViagemControleEntrega.Todas), options: EnumStatusViagemControleEntrega.obterOpcoes(), def: EnumStatusViagemControleEntrega.Todas });
    this.SituacaoCarga = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.SituacaoCarga, val: ko.observable(EnumSituacoesCarga.NaLogistica), options: opcoesSituacaoCarga, def: EnumSituacoesCarga.NaLogistica });
    this.FiltroTendenciaEntrega = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.FiltroTendenciaEntrega, val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Todos), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Todos });

    this.FiltroTendenciaColeta = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.FiltroTendenciaColeta, val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Todos), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Todos });

    this.StatusDaViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EtapaDoMonitoramento.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });

    this.DataCarregamentoInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioDoCarregamento.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCarregamentoFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });

    this.DataPrevisaoInicioViagemInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPlanejadaInicioViagem.getFieldDescription(), issue: 60770, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataPrevisaoInicioViagemFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), issue: 60770, getType: typesKnockout.date });
    this.DataCriacaoCargaInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataCriacaoCarga.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCriacaoCargaFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date });
    this.DataPrevisaoEntregaInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoEntrega.getFieldDescription(), getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date });
    this.DataEntregaInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEntrega.getFieldDescription(), getType: typesKnockout.date });
    this.DataEntregaFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date });

    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Veiculos, idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "Placa", propCodigo: "Codigo" } });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista, idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalColeta = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.LocalColeta, idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.LocalEntrega, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CidadeColeta = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CidadeColeta, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CidadeEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CidadeEntrega, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DestinatarioPedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.DestinatarioPedido, idBtnSearch: guid(), visible: ko.observable(true) });
    this.RemetentePedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.RemetentePedido, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Recebedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.FilialVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoCarga.getFieldDescription(), issue: 53, idBtnSearch: guid() });
    this.TipoDocumentoTransporte = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo Documento Transporte", idBtnSearch: guid() });

    this.MonitoramentoStatus = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SituacaoDoMonitoramento.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(EnumMonitoramentoStatus.Iniciado), options: EnumMonitoramentoStatus.obterOpcoes(), def: EnumMonitoramentoStatus.Iniciado, placeholder: Localization.Resources.Cargas.ControleEntrega.StatusMonitoramento });

    this.DataInicioViagemInicial.dateRangeLimit = this.DataInicioViagemFinal;
    this.DataInicioViagemFinal.dateRangeInit = this.DataInicioViagemInicial;

    this.DataCarregamentoInicial.dateRangeLimit = this.DataCarregamentoFinal;
    this.DataCarregamentoFinal.dateRangeInit = this.DataCarregamentoInicial;

    this.DataPrevisaoInicioViagemInicial.dateRangeLimit = this.DataPrevisaoInicioViagemFinal;
    this.DataPrevisaoInicioViagemFinal.dateRangeInit = this.DataPrevisaoInicioViagemInicial;

    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;

    this.DataEntregaInicial.dateRangeLimit = this.DataEntregaFinal;
    this.DataEntregaFinal.dateRangeInit = this.DataEntregaInicial;

    this.DataCriacaoCargaInicial.dateRangeLimit = this.DataCriacaoCargaFinal;
    this.DataCriacaoCargaFinal.dateRangeInit = this.DataCriacaoCargaInicial;

    this.ExibirSomenteCargasComVeiculo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TracaoDefinido, getType: typesKnockout.bool, val: ko.observable(true) });
    this.ExibirSomenteCargasComChamadoAberto = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AtendimentoAberto, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasComReentrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PedidoReentrega, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasUsuarioMonitora = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CargasUsuarioMonitora, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasMotoristaMobile = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasMotoristaMobile, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasComPesquisaDeDesembarquePendente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PesquisaDeDesembarquePendente, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.ExibirSomenteCargasEmAtraso = PropertyEntity({ text: "Cargas em Atraso", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasCriticas = PropertyEntity({ text: "Cargas criticas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasAlertaMonitoramentoAberto = PropertyEntity({ text: "Alertas de monitoramento em Aberto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasAlertaMonitoramentoEmTratativa = PropertyEntity({ text: "Alertas de monitoramento em Tratativas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasSemAlertas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.VeiculoNoRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TracaoRaio, getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.ExibirSomenteCargasFarolEspelhamentoOnline = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null) });
    this.ExibirSomenteCargasFarolEspelhamentoOffline = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null) });

    this.PossuiRecebedor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PossuiRecebedor.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.PossuiExpedidor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PossuiExpedidor.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.PreTrip = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CargasPreTrip, val: ko.observable(EnumPreTrip.Todas), options: EnumPreTrip.obterOpcoes(), def: EnumPreTrip.Todas, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaAppTrizy) });

    this.DataAgendamentoPedidoInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialAgendamentoPedido.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), def: "" });
    this.DataAgendamentoPedidoFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalAgendamentoPedido.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), def: "" });
    this.DataColetaPedidoInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialColetaPedido.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), def: "" });
    this.DataColetaPedidoFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalColetaPedido.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), def: "" });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NumeroPedidoCliente.getFieldDescription(), col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Carga, val: ko.observable() });
    this.ClientesComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.ClienteComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CanalVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoMercadoria = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria.getFieldDescription() });
    this.EquipeVendas = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.EquipeVendas.getFieldDescription() });
    this.EscritorioVenda = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.EscritorioVenda.getFieldDescription() });
    this.RotaFrete = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Cargas.ControleEntrega.RotaFrete.getFieldDescription() });
    this.Mesoregiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Mesoregiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Regiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Regiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Matriz = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.Matriz.getFieldDescription() });
    this.GrupoDePessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Parqueada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Parqueada.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CanalEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicioAbate = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioAbate.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataFimAbate = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataFimAbate.getFieldDescription(), getType: typesKnockout.dateTime });
    
    this.DataInicioAbate.dateRangeLimit = this.DataFimAbate;
    this.DataFimAbate.dateRangeInit = this.DataInicioAbate;

    this.FiltroPesquisa = PropertyEntity({ val: ko.observable(""), def: "" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Todos;
            ObterResumoAcompanhamentoCargas();
            BuscarCargasAcompanhamento(1, false, false, false, false, false).then(function () {
                AplicarConfigWidget();
                ObterMensagensNaoLidasCards();
                loadCargasNoMapa();
            });
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AplicarFiltros, idGrid: guid(), visible: ko.observable(true)
    });

    this.Limpar = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Todos;
            LimparCampos(_pesquisaAcompanhamentoCarga);
        }, type: types.event, idGrid: guid(), visible: ko.observable(true)
    });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.AcompanhamentoCarga,
        callbackRetornoPesquisa: function () {
            $("#" + _pesquisaAcompanhamentoCarga.StatusViagemControleEntrega.id).trigger("change");
            $("#" + _pesquisaAcompanhamentoCarga.FiltroPesquisa.id).trigger("change");
            $("#" + _pesquisaAcompanhamentoCarga.MonitoramentoStatus.id).trigger("change");
        }
    });

    this.TipoAlerta = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoAlerta.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.AcompanhamentoCarga, _pesquisaAcompanhamentoCarga) }, type: types.event, text: Localization.Resources.Gerais.Geral.ConfiguracaoDeFiltros, visible: ko.observable(true) });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-chevron-down");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-chevron-up");
            }
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.BuscaAvancada, idFade: guid(), icon: ko.observable("fal fa-chevron-down"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ResumoAcompanhamentoCarga = function () {
    this.resumoTodas = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.resumoNaoIniciada = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.resumoEmViagem = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.resumoFinalizadas = PropertyEntity({ type: types.string, val: ko.observable("") });

    this.atendimentosPendentes = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.atendimentosTratativas = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.atendimentosAtrasados = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.atendimentosConcluidos = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.alertasEmAberto = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.alertasEmTratativa = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.alertasFinalizados = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.tendenciaEntregaNenhum = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.tendenciaEntregaAdiantado = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.tendenciaEntregaNoPrazo = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.tendenciaEntregaAtraso = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.tendenciaEntregaAtrasado = PropertyEntity({ type: types.string, val: ko.observable("") });
    this.quantidadeFarolEspelhamentoOnline = PropertyEntity({ type: types.string, val: ko.observable(""), porcentagem: ko.observable("") });
    this.quantidadeFarolEspelhamentoOffline = PropertyEntity({ type: types.string, val: ko.observable(""), porcentagem: ko.observable("") });

    this.ClickTodas = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
            _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.Todas;

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });

            if ($("#view-1").hasClass("active")) {
                $("#view-1").removeClass("active");
            } else {
                $("#view-1").addClass("active");
                $("#view-11").removeClass("active");
                $("#view-2").removeClass("active");
                $("#view-3").removeClass("active");
            }
            
            desactiveBotoesAtendimento();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickNaoIniciada = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($("#view-11").hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
            _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.NaoIniciada;
            $("#view-1").removeClass("active");
            $("#view-11").addClass("active");
            $("#view-2").removeClass("active");
            $("#view-3").removeClass("active");
            desactiveBotoesAtendimento();
            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
           
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickEmViagem = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($("#view-2").hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
            _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.EmViagem;
            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
            $("#view-1").removeClass("active");
            $("#view-11").removeClass("active");
            $("#view-2").addClass("active");
            $("#view-3").removeClass("active");
            desactiveBotoesAtendimento();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickFinalizadas = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($("#view-3").hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
            _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.Finalizadas;
            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
            $("#view-1").removeClass("active");
            $("#view-11").removeClass("active");
            $("#view-2").removeClass("active");
            $("#view-3").addClass("active");
            desactiveBotoesAtendimento();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAtendimentoPendente = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-pendentes').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Pendente;

            $("#view-pendentes").addClass("active");
            $("#view-emtratativa").removeClass("active");
            $("#view-atrasadas").removeClass("active");
            $("#view-concluidas").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });

        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAtendimentoTratativa = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-emtratativa').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Tratativa;

            $("#view-emtratativa").addClass("active");
            $("#view-pendentes").removeClass("active");
            $("#view-atrasadas").removeClass("active");
            $("#view-concluidas").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });

        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAtendimentoAtrasado = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            if ($('#view-atrasadas').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Atrasada;

            $("#view-atrasadas").addClass("active");
            $("#view-pendentes").removeClass("active");
            $("#view-emtratativa").removeClass("active");
            $("#view-concluidas").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });

        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAtendimentoConcluido = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-concluidas').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Concluida;

            $("#view-pendentes").removeClass("active");
            $("#view-emtratativa").removeClass("active");
            $("#view-atrasadas").removeClass("active");
            $("#view-concluidas").addClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickNenhum = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-nenhum').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Nenhum;

            $("#view-nenhum").addClass("active");
            $("#view-adiantado").removeClass("active");
            $("#view-noprazo").removeClass("active");
            $("#view-entrega-atraso").removeClass("active");
            $("#view-atrasado").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickTendenciaAdiantado = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-adiantado').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Adiantado;

            $("#view-adiantado").addClass("active");
            $("#view-nenhum").removeClass("active");
            $("#view-noprazo").removeClass("active");
            $("#view-entrega-atraso").removeClass("active");
            $("#view-atrasado").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickNoPrazo = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-noprazo').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.NoPrazo;

            $("#view-noprazo").addClass("active");
            $("#view-adiantado").removeClass("active");
            $("#view-nenhum").removeClass("active");
            $("#view-entrega-atraso").removeClass("active");
            $("#view-atrasado").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickTendenciaAtraso = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-entrega-atraso').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.TendenciaEntrega;

            $("#view-entrega-atraso").addClass("active");
            $("#view-adiantado").removeClass("active");
            $("#view-nenhum").removeClass("active");
            $("#view-noprazo").removeClass("active");
            $("#view-atrasado").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickTendenciaAtrasado = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-atrasado').hasClass("active")) {
                DesabilitarFiltro();
                return;
            } 

            _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Atrasado;

            $("#view-atrasado").addClass("active");
            $("#view-adiantado").removeClass("active");
            $("#view-nenhum").removeClass("active");
            $("#view-noprazo").removeClass("active");
            $("#view-entrega-atraso").removeClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAlertasEmAberto = PropertyEntity({
        eventClick: function (e) {
            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoAberto.val(true);

            if ($('#view-abertos').hasClass("active")) {
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoAberto.val(false);
                DesabilitarFiltro();
                return;
            } 

            if ($('#view-finalizadas').hasClass('active')) {
                $("#view-finalizadas").removeClass("active");
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasSemAlertas.val(false);
            }
            else if ($('#view-em-tratativa').hasClass('active')) {
                $("#view-em-tratativa").removeClass("active");
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoEmTratativa.val(false);
            }

            $("#view-abertos").addClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAlertasEmTratativa = PropertyEntity({
        eventClick: function (e) {
            if ($('#view-em-tratativa').hasClass("active")) {
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoEmTratativa.val(false);
                DesabilitarFiltro();
                return;
            } 

            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoEmTratativa.val(true);

            if ($('#view-finalizadas').hasClass('active')) {
                $("#view-finalizadas").removeClass("active");
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasSemAlertas.val(false);
            }
            else if ($('#view-abertos').hasClass('active')) {
                $("#view-abertos").removeClass("active");
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoAberto.val(false);
            }

            $("#view-em-tratativa").addClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickAlertasFinalizados = PropertyEntity({
        eventClick: function (e) {

            if ($('#view-finalizadas').hasClass("active")) {
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasSemAlertas.val(false);
                DesabilitarFiltro();
                return;
            }

            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoAberto.val(false);
            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasSemAlertas.val(true);

            if ($('#view-em-tratativa').hasClass('active')) {
                $("#view-em-tratativa").removeClass("active");
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoEmTratativa.val(false);
            }
            else if ($('#view-abertos').hasClass('active')) {
                $("#view-abertos").removeClass("active");
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasAlertaMonitoramentoAberto.val(false);
            }

            $("#view-finalizadas").addClass("active");

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickFarolEspelhamentoOnline = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-espelhamento-online').hasClass("active")) {
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOnline.val(null);
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOffline.val(null);
                DesabilitarFiltro();
                return;
            }

            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOnline.val(true);
            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOffline.val(null);

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });

            $("#view-espelhamento-offline").removeClass("active");
            $("#view-espelhamento-online").addClass("active");

            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOffline.val(null);
            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOnline.val(null);

            desactiveBotoesAtendimento();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ClickFarolEspelhamentoOffline = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            if ($('#view-espelhamento-offline').hasClass("active")) {
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOnline.val(null);
                _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOffline.val(null);
                DesabilitarFiltro();
                return;
            }

            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOffline.val(true);
            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOnline.val(null);

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
                loadCargasNoMapa();
            });

            $("#view-espelhamento-online").removeClass("active");
            $("#view-espelhamento-offline").addClass("active"); 

            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOffline.val(null);
            _pesquisaAcompanhamentoCarga.ExibirSomenteCargasFarolEspelhamentoOnline.val(null);

            desactiveBotoesAtendimento();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });


    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            Global.abrirModal("knockoutPesquisaAcompanhamentoCarga");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.MostrarCards = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            controlarExibicoes(true);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Cards"
    });

    this.MostrarMap = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            controlarExibicoes(false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Mapa"
    });

    this.ExibirConfiguracaoWidget = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            Global.abrirModal("knockoutConfigWidgetAcompanhamentoCarga");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var AcompanhamentoCarga = function () {
    this.Cargas = ko.observableArray([]);
}

var Paradas = function () {
    this.Paradas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
}

var CardCarga = function (data) {
    if (data == null)
        data = {};

    this.Data = data;
    this.CodigoCarga = PropertyEntity({ val: ko.observable(data["CodigoCarga"]) });
    this.CargaEmbarcador = PropertyEntity({ val: ko.observable(data["CargaEmbarcador"]) });
    this.CargaTransbordo = PropertyEntity({ val: ko.observable(data["CargaTransbordo"]) });
    this.Veiculo = PropertyEntity({ val: ko.observable(data["Veiculo"]) });
    this.DataCriacaoCargaFormatada = PropertyEntity({ val: ko.observable(data["DataCriacaoCargaFormatada"]) });
    this.DataPrevisaoTerminoCargaFormatada = PropertyEntity({ val: ko.observable(data["DataPrevisaoTerminoCargaFormatada"]) });
    this.Veiculos = PropertyEntity({ val: ko.observable(data["Veiculos"]), visible: ko.observable(true) });
    this.VeiculoTracao = PropertyEntity({ val: ko.observable(data["VeiculoTracao"]), visible: ko.observable(true) });
    this.Reboques = PropertyEntity({ val: ko.observable(data["Reboques"]), visible: ko.observable(true) });
    this.NumeroFrotaReboques = PropertyEntity({ val: ko.observable(data["NumeroFrotaReboques"]), visible: ko.observable(false) });
    this.DataInicioViagemFormatada = PropertyEntity({ val: ko.observable(data["DataInicioViagemFormatada"]), visible: ko.observable(true) });
    this.DataInicioViagemPrevistaFormatada = PropertyEntity({ val: ko.observable(data["DataInicioViagemPrevistaFormatada"]) });
    this.DataCarregamentoCargaFormatada = PropertyEntity({ val: ko.observable(data["DataCarregamentoCargaFormatada"]) });
    this.DataFimViagemFormatada = PropertyEntity({ val: ko.observable(data["DataFimViagemFormatada"]) });
    this.DataFimViagemPrevistaFormatada = PropertyEntity({ val: ko.observable(data["DataFimViagemPrevistaFormatada"]) });
    this.DataPrevisaoChegadaPlantaFormatada = PropertyEntity({ val: ko.observable(data["DataPrevisaoChegadaPlantaFormatada"]) });
    this.DataPreViagemInicioFormatada = PropertyEntity({ val: ko.observable(data["DataPreViagemInicioFormatada"]) });
    this.DataPreViagemFimFormatada = PropertyEntity({ val: ko.observable(data["DataPreViagemFimFormatada"]) });
    this.HabilitarPreViagemTrizy = PropertyEntity({ val: ko.observable(data["HabilitarPreViagemTrizy"]) });
    this.ChegadaPlantaAtraso = PropertyEntity({ val: ko.observable(data["ChegadaPlantaAtraso"]) });
    this.TempoAtrasoChegadaPlanta = PropertyEntity({ val: ko.observable(data["TempoAtrasoChegadaPlanta"]) });
    this.CargaCancelada = PropertyEntity({ val: ko.observable(data["CargaCancelada"]) });
    this.PercentualViagem = PropertyEntity({ val: ko.observable(data["PercentualViagem"]) });
    this.NomeTransportador = PropertyEntity({ val: ko.observable(data["NomeTransportador"]) });
    this.TipoOperacao = PropertyEntity({ val: ko.observable(data["TipoOperacao"]), visible: ko.observable(true) });
    this.NomeAnalistaResponsavel = PropertyEntity({ val: ko.observable(data["NomeAnalistaResponsavel"]) });
    this.Motoristas = PropertyEntity({ val: ko.observable(data["Motoristas"]), visible: ko.observable(true) });
    this.NumeroMotorista = PropertyEntity({ val: ko.observable(data["NumeroMotorista"]) });
    this.ConclusaoViagem = PropertyEntity({ val: ko.observable(data["ConclusaoViagem"]) });
    this.ConclusaoEntregas = PropertyEntity({ val: ko.observable(data["ConclusaoEntregas"]) });
    this.ConclusaoColetas = PropertyEntity({ val: ko.observable(data["ConclusaoColetas"]) });
    this.ValorTotalNF = PropertyEntity({ val: ko.observable(data["ValorTotalNF"]), visible: ko.observable(true) });
    this.PesoTotalNF = PropertyEntity({ val: ko.observable(data["PesoTotalNF"]), visible: ko.observable(true) });
    this.PesoBruto = PropertyEntity({ val: ko.observable(data["PesoTotalCarga"]), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ val: ko.observable(data["Filial"]), visible: ko.observable(true) });
    this.AnotacoesCard = PropertyEntity({ val: ko.observable(data["AnotacoesCard"]), visible: ko.observable(false) });
    this.DataPrevisaoInicioViagem = PropertyEntity({ val: ko.observable(data["DataPrevisaoInicioViagem"]), visible: ko.observable(false) });
    this.DataPrevistaProximaEntrega = PropertyEntity({ val: ko.observable(data["DataPrevistaProximaEntrega"]), visible: ko.observable(true) });
    this.DataReprogramadaProximaEntrega = PropertyEntity({ val: ko.observable(data["DataReprogramadaProximaEntrega"]), visible: ko.observable(false) });
    this.ProximaEntregaAtraso = PropertyEntity({ val: ko.observable(data["ProximaEntregaAtraso"]) });
    this.TempoAtrasoProximaEntrega = PropertyEntity({ val: ko.observable(data["TempoAtrasoProximaEntrega"]) });
    this.StatusLoger = PropertyEntity({ val: ko.observable(data["StatusLoger"]) });
    this.CodigoRemetente = PropertyEntity({ val: ko.observable(data["CodigoRemetente"]) });
    this.CodigoTransportador = PropertyEntity({ val: ko.observable(data["CodigoTransportador"]) });
    this.Peso = PropertyEntity({ val: ko.observable(data["PesoTotalCarga"]) });
    this.Origens = PropertyEntity({ val: ko.observable(data["Origens"]) });
    this.Destinos = PropertyEntity({ val: ko.observable(data["Destinos"]) });
    this.PossuiMonitoramento = PropertyEntity({ val: ko.observable(data["PossuiMonitoramento"]) });
    this.DescricaoStatusViagem = PropertyEntity({ val: ko.observable(data["DescricaoStatusViagem"]) });
    this.ProximoDestino = PropertyEntity({ val: ko.observable(data["ProximoDestino"]), visible: ko.observable(true) });
    this.ProximaCidadeDestino = PropertyEntity({ val: ko.observable(data["ProximaCidadeDestino"]), visible: ko.observable(false) });
    this.DistanciaAteDestino = PropertyEntity({ val: ko.observable(data["DistanciaAteDestino"]) });
    this.DistanciaPrevista = PropertyEntity({ val: ko.observable(data["DistanciaPrevista"]) });
    this.CodigoMonitoramento = PropertyEntity({ val: ko.observable(data["CodigoMonitoramento"]) });
    this.DistanciaRealizada = PropertyEntity({ val: ko.observable(data["DistanciaRealizada"]) });
    this.NivelGPS = PropertyEntity({ val: ko.observable(data["NivelGPS"]) });
    this.NivelBateria = PropertyEntity({ val: ko.observable(data["NivelBateria"]) });
    this.ExibirNivelBateria = PropertyEntity({ val: ko.observable(data["ExibirNivelBateria"]) });
    this.MonitoramentoCargaCritica = PropertyEntity({ val: ko.observable(data["MonitoramentoCargaCritica"]) });
    this.DataUltimaPosicao = PropertyEntity({ val: ko.observable(data["DataUltimaPosicao"]) });
    this.onLine = PropertyEntity({ val: ko.observable(data["onLine"]) });
    this.PercentualCargaEntregue = PropertyEntity({ val: ko.observable(data["PercentualCargaEntregue"]) });
    this.TotalDeEntregas = PropertyEntity({ val: ko.observable(data["TotalDeEntregas"]) });
    this.TotalDeEntregasEntregues = PropertyEntity({ val: ko.observable(data["TotalDeEntregasEntregues"]) });
    this.TotalDeColetas = PropertyEntity({ val: ko.observable(data["TotalDeColetas"]) });
    this.TotalDeColetasColetadas = PropertyEntity({ val: ko.observable(data["TotalDeColetasColetadas"]) });
    this.PossuiAlerta = PropertyEntity({ val: ko.observable(data["PossuiAlerta"]), visible: ko.observable(true) });
    this.PossuiAlertaVelocidade = PropertyEntity({ val: ko.observable(data["PossuiAlertaVelocidade"]) });
    this.PossuiAlertaInicioViagemSemDocumentacao = PropertyEntity({ val: ko.observable(data["PossuiAlertaInicioViagemSemDocumentacao"]) });
    this.PossuiAlertaParadaExcessiva = PropertyEntity({ val: ko.observable(data["PossuiAlertaParadaExcessiva"]) });
    this.PossuiAlertaTemperaturaForaFaixa = PropertyEntity({ val: ko.observable(data["PossuiAlertaTemperaturaForaFaixa"]) });
    this.PossuiAlertaParadaNaoProgramada = PropertyEntity({ val: ko.observable(data["PossuiAlertaParadaNaoProgramada"]) });
    this.DataUltimoAlerta = PropertyEntity({ val: ko.observable(data["DataUltimoAlerta"]) });
    this.TipoUltimoAlerta = PropertyEntity({ val: ko.observable(data["TipoUltimoAlerta"]) });
    this.PossuiMensagemNaoLida = PropertyEntity({ val: ko.observable(false) });
    this.CodigoCargaEspelhadaComMonitoramentoAtivo = PropertyEntity({ val: ko.observable(data["CodigoCargaEspelhadaComMonitoramentoAtivo"]) });
    this.CargaEspelhadaEmbarcador = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.DataCarregamentoEspelhada = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.PossuiMonitoramentoAtivoProVeiculoEmOutraCarga = PropertyEntity({ val: ko.observable(data["PossuiMonitoramentoAtivoProVeiculoEmOutraCarga"]) });
    this.NovoCard = PropertyEntity({ val: ko.observable(false) });
    this.DescricaoRastreador = PropertyEntity({ val: ko.observable(data["DescricaoRastreador"]) });
    this.LeadTimeTransportador = PropertyEntity({ val: ko.observable(data["LeadTimeTransportadorProximaEntrega"]), visible: ko.observable(true) });
    this.CanalVenda = PropertyEntity({ val: ko.observable(data["CanalVenda"]), visible: ko.observable(false) });
    this.ModalTransporte = PropertyEntity({ val: ko.observable(data["ModalTransporte"]), visible: ko.observable(false) });
    this.Mesoregiao = PropertyEntity({ val: ko.observable(data["Mesoregiao"]), visible: ko.observable(false) });
    this.Regiao = PropertyEntity({ val: ko.observable(data["Regiao"]), visible: ko.observable(false) });  
    this.PossuiAlertaExibirTela = PropertyEntity({ val: ko.observable(data["PossuiAlertaExibirTela"]) });
    this.DescricaoTipoUltimoAlertaExibirTela = PropertyEntity({ val: ko.observable(data["DescricaoTipoUltimoAlertaExibirTela"]) });
    this.ExibirDescricaoAlerta = PropertyEntity({ val: ko.observable(data["ExibirDescricaoAlerta"]) });
    this.ExibirDataeHoraGeracaoAlerta = PropertyEntity({ val: ko.observable(data["ExibirDataeHoraGeracaoAlerta"]) });
    this.IconeUltimoAlertaExibirTela = PropertyEntity({ val: ko.observable(data["IconeUltimoAlertaExibirTela"]) });
    this.TipoUltimoAlertaCarga = PropertyEntity({ val: ko.observable(data["TipoUltimoAlertaCarga"]) });
    this.TipoUltimoAlertaMonitoramento = PropertyEntity({ val: ko.observable(data["TipoUltimoAlertaMonitoramento"]) });
    this.StatusUltimoAlertaMonitoramento = PropertyEntity({ val: ko.observable(data["StatusUltimoAlertaMonitoramento"]) });
    this.NomeResponsavelUltimoAlertaMonitoramento = PropertyEntity({ val: ko.observable(data["NomeResponsavelUltimoAlertaMonitoramento"]) });
    this.DescricaoResponsavelOUTempoAberto = PropertyEntity({ val: ko.observable("") });
    this.DescricaoStatusUltimoAlertaMonitoramento = PropertyEntity({ val: ko.observable(data["DescricaoStatusUltimoAlertaMonitoramento"]) });
    this.QuantidadeAlertasCarga = PropertyEntity({ val: ko.observable(data["QuantidadeAlertasCarga"]) });
    this.CodigoUltimoAlerta = PropertyEntity({ val: ko.observable(data["CodigoUltimoAlerta"]) });
    this.CargaFixadaControleCargas = PropertyEntity({ val: ko.observable(data["CargaFixadaControleCargas"]) });
    this.Latitude = PropertyEntity({ val: ko.observable(data["Latitude"]) });
    this.Longitude = PropertyEntity({ val: ko.observable(data["Longitude"]) });
    this.Entregas = PropertyEntity({ type: types.local, val: ko.observableArray([]) });

    this.ChamadosPendentes = PropertyEntity({ val: ko.observable(data["ChamadosPendentes"]) });
    this.ChamadosEmTratativa = PropertyEntity({ val: ko.observable(data["ChamadosEmTratativa"]) });
    this.ChamadosAtrasados = PropertyEntity({ val: ko.observable(data["ChamadosAtrasados"]) });
    this.ChamadosConcluidos = PropertyEntity({ val: ko.observable(data["ChamadosConcluidos"]) });

    this.Cor = PropertyEntity({ val: ko.observable(data["Cor"]) });
    this.TipoModal = PropertyEntity({ val: ko.observable(data["TipoModal"]) });
    this.PedidoEmOutrasCargas = PropertyEntity({ val: ko.observable(data["PedidoEmOutrasCargas"]), visible: ko.observable(true) });
    this.TendenciaEntrega = PropertyEntity({ val: ko.observable(data["TendenciaProximaEntrega"]), visible: ko.observable(false) });
    this.TendenciaColeta = PropertyEntity({ val: ko.observable(data["TendenciaProximaColeta"]), visible: ko.observable(false) });
    this.UltimaEntregaRealizadaNoPrazoDescricao = PropertyEntity({ val: ko.observable(data["UltimaEntregaRealizadaNoPrazoDescricao"]), visible: ko.observable(false) });
    this.UltimaColetaRealizadaNoPrazoDescricao = PropertyEntity({ val: ko.observable(data["UltimaColetaRealizadaNoPrazoDescricao"]), visible: ko.observable(false) });
    this.ProximaEntregaIsColeta = PropertyEntity({ val: ko.observable(data["ProximaEntregaIsColeta"]), visible: ko.observable(true) });


    this.DataAgendamentoPedidoFormatada = PropertyEntity({ val: ko.observable(data["DataAgendamentoPedidoFormatada"]), visible: ko.observable(true) });
    this.DataCarregamentoPedidoFormatada = PropertyEntity({ val: ko.observable(data["DataCarregamentoPedidoFormatada"]), visible: ko.observable(true) });
    this.MatrizComplementar = PropertyEntity({ val: ko.observable(data["MatrizComplementar"]), visible: ko.observable(true) });
    this.EscritorioVendasComplementar = PropertyEntity({ val: ko.observable(data["EscritorioVendasComplementar"]), visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ val: ko.observable(data["NumeroPedidoCliente"]), visible: ko.observable(true) });

    this.PossuiPosicao = PropertyEntity({ val: ko.observable(data["PossuiPosicao"]), visible: ko.observable(true) });

    this.PermiteDelegarMonitoramento = PropertyEntity({ val: ko.observable(true) }); //_operadorLogistica.OperadorSupervisor

    this.InicioViagemForaRaio = PropertyEntity({ val: ko.observable(data["InicioViagemForaRaio"]), visible: ko.observable(false) });
    this.ImagemForaRaio = PropertyEntity({ val: ko.observable(data["ImagemForaRaio"]), visible: ko.observable(false) });
    this.EhAlertaParada = PropertyEntity({ val: ko.observable(data["EhAlertaParada"]), visible: ko.observable(false) });

    this.UtilizaAppTrizy = PropertyEntity({ val: ko.observable(data["UtilizaAppTrizy"]) });

    this.InicioViagem = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirInicioViagemClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.FimViagem = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirFimViagemClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.IniciarPreTrip = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirInicioPreTripClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.FinalizarPreTrip = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirFimPreTripClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.mensagemChat = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined || !this.UtilizaAppTrizy.val()) return;
            abrirChatMensagemClick(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Chat, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true),
        icon: ko.observable("../../../../Content/TorreControle/Icones/gerais/mensagem-desabilitada.svg"), enable: this.UtilizaAppTrizy.val()
    });

    this.MonitoramentoAtivoProVeiculoEmOutraCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            detalhesCargaEspelhadaClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.DadosCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            if (e.CodigoCarga.val() == _cardAtivo) {
                $("#DivDetalhesEntrega").fadeOut("fast");
                $(".legenda-mapaCards").hide();
                $("#card_" + _cardAtivo).removeClass("active-card");
                $("#card_" + _cardAtivo).css("box-shadow", "0 3px 10px #ccc");
                _cardAtivo = 0;
            }
            else {
                for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {
                    if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == e.CodigoCarga.val()) {
                        viewDetalhesCargaClick(_cardAcompanhamentoCarga.Cargas()[i]);
                        break;
                    }
                }
            }
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.AlertaClick = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            alertaCardClick(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.DadosCarga, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.VisualizarDadosCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirDadosCargaClick();
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.DadosCarga, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.VisualizarDetalhesPedidos = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirModalDetalhesPedidoClick(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.DetalhesDosPedidos, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.VisualizarNoMapa = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            dadosMapaClick();
        }, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.VisualizarMapa), idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.AdicionarEvento = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            adicionarEventoCarga();
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AdicionarEventos, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.RaioXCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            abrirRaioXCarga(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.RaioXDaCarga, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.Assumir = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            assumirMonitoramentoCarga(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Assumir, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.Anotacoes = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            atribuirAnotacoesCargaClick(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Anotacoes, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.OcorrenciaFrete = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            abrirModalOcorrencia(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.OcorrenciaFrete, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.AdicionarPedidoReentrega = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            pedidoReentregaClick();
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AdicionarPedidoReentrega, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.DownloadBoletimEmbarque = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            downloadRelatorioBoletimViagem();
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.BoletimEmbarque, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.FixarPin = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            fixarPinCardClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.NovoCard = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            removerTagNovoCard(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.VisualizarHistoricoMonitoramento = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            visualizarHistoricosClick();
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Historicos, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.DetalhesMonitoramento = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            visualizarDetalhesMonitoramentoClick();
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.DetalhesMonitoramento, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.DetalhesTorre = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            visualizarDetalhesTorreClick();
        }, type: types.event, text: Localization.Resources.Logistica.Monitoramento.DetalhesTorre, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.PedidoEmOutrasCargasClick = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            BuscarPedidoOutraCarga(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ExibirLegenda = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: ExibirLegendaClick, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ExibirLegenda), visible: ko.observable(true) });

    this.StatusAcompanhamento = ko.computed(() => {
        let status = EnumStatusAcompanhamento.SemMonitoramento;
        let hover = EnumStatusAcompanhamento.obterDescricao(status);

        if (this.PossuiMonitoramento.val()) {
            status = EnumStatusAcompanhamento.ComMonitoramentoSemPosicao;

            if (this.PossuiPosicao.val()) {
                if (!_CONFIGURACAO_TMS.TempoSemPosicaoParaVeiculoPerderSinal)
                    status = EnumStatusAcompanhamento.ComMonitoramentoComUmaPosicao;
                else status = this.onLine.val() ?
                    EnumStatusAcompanhamento.ComPosicaoRecebidaNoTempoConfigurado : EnumStatusAcompanhamento.SemPosicaoRecebidaNoTempoConfigurado;
            }
        } else
            return '<div class="no-signal" title="' + hover + '"></div>';

        let icone = ObterIconeStatusTracking(status, 20);
        hover = EnumStatusAcompanhamento.obterDescricao(status);

        return '<div class="mutable" title="' + hover + '">' + icone + '</div>';
    }, this)

    this.FecharDetalhesCarga = PropertyEntity({ eventClick: fecharDetalhes, type: types.event, idGrid: guid() });

    this.AssumirAlerta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            AssumirAlertaCardCargaClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.DeixarAlerta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            DeixarAlertaCardCargaClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.ResolverAlerta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            ResolverAlertaCardCargaClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
    this.DetalhesAlerta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            DetalhesAlertaCardCargaClick(e);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.AlterarMotoristaDaCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            abrirModalAlterarMotoristaDaCarga(e);
        }, type: types.event, text: Localization.Resources.Cargas.Carga.AlterarMotorista, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    this.AlterarVeiculoDaCarga = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            abrirModalAlterarVeiculoDaCarga(e);
        }, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AlterarTracao, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), visibleVisualizacaoRapida: ko.observable(false)
    });

    carregarDadosCargaEspelhada(this);
}


var AlterarMotoristaAcompanhamentoCarga = function () {
    this.SubTitulo = PropertyEntity({ val: ko.observable("Altere o motorista da carga a partir da pesquisa de Motoristas do Transportador {1}.") });

    this.CodigoCarga = PropertyEntity({ type: types.int, val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: "Transportador", type: types.int, val: ko.observable("") });
    this.CodigoTransportador = PropertyEntity({ type: types.int, val: ko.observable("") });

    this.NomeMotorista = PropertyEntity({ text: "Motorista", type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.CPFMotorista = PropertyEntity({ text: "CPF", type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.GridMotoristas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.GridIntegracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Pesquisar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pesquisar, eventClick: pesquisarMotoristasAlterarMotoristaAcompanhamentoCarga, type: types.event, visible: ko.observable(true) });
    this.Alterar = PropertyEntity({ text: "Alterar e Integrar", eventClick: confirmarAlterarMotoristaAcompanhamentoCarga, type: types.event, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: fecharModalAlterarMotoristaDaCarga, type: types.event, visible: ko.observable(true) });
    this.RecarregarIntegracoes = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Atualizar, eventClick: recarregarIntegracoesAlterarMotoristaAcompanhamentoCarga, type: types.event, visible: ko.observable(true) });
}

var AlterarVeiculoAcompanhamentoCarga = function () {
    this.SubTitulo = PropertyEntity({ val: ko.observable("Altere os veículos a partir da pesquisa de Veículos do Transportador: {1}.") });

    this.CodigoCarga = PropertyEntity({ type: types.int, val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: "Transportador", type: types.int, val: ko.observable("") });

    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.PlacaVeiculo = PropertyEntity({ text: "Placa", type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.GridVeiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.GridIntegracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.Pesquisar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pesquisar, eventClick: pesquisarVeiculosAlterarVeiculoAcompanhamentoCarga, type: types.event, visible: ko.observable(true) });

    this.CodigoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.CPFMotorista = PropertyEntity({ text: "CPF", type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.GridMotoristas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.SubTituloMotorista = PropertyEntity({ val: ko.observable("Altere o motorista a partir da pesquisa de Motoristas do Transportador: {1}.") });
    this.PesquisarMotorista = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pesquisar, eventClick: pesquisarMotoristasAlterarVeiculo, type: types.event, visible: ko.observable(true) });

    this.Alterar = PropertyEntity({ text: "Alterar e Integrar", eventClick: confirmarAlterarVeiculoAcompanhamentoCarga, type: types.event, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: fecharModalAlterarVeiculoDaCarga, type: types.event, visible: ko.observable(true) });
    this.RecarregarIntegracoes = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Atualizar, eventClick: recarregarIntegracoesAlterarVeiculoAcompanhamentoCarga, type: types.event, visible: ko.observable(true) });
    this.ExibirAlterarMotoristaCarga = PropertyEntity({ text: "Deseja alterar o motorista da carga?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AlterarVeiculoAlterarMotoristaVisibleDiv = ko.computed(() => {

        var exibirAlterarMotoristaCarga = this.ExibirAlterarMotoristaCarga.val();

        if (exibirAlterarMotoristaCarga) {
            limparMotoristaAlterarVeiculoDaCarga();
        }

        return exibirAlterarMotoristaCarga;
    }, this)
}

function carregarDadosCargaEspelhada(card) {
    if (card.PossuiMonitoramentoAtivoProVeiculoEmOutraCarga.val()) {
        var codigoCarga = card.CodigoCargaEspelhadaComMonitoramentoAtivo.val();
        if (codigoCarga > 0) {
            executarReST("AcompanhamentoCarga/DetalhesCargaEspelhada", { CodigoCarga: codigoCarga }, function (arg) {
                if (arg.Success) {
                    var data = arg.Data;
                    if (data !== false) {
                        card.CargaEspelhadaEmbarcador.val(arg.Data.CargaEmbarcador);
                        card.DataCarregamentoEspelhada.val(arg.Data.DataCarregamentoCarga);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, arg.Msg);
                }
            });
        }
    }
}

let ModalAnotacoesCarga = function (data) {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) })
    this.AnotacoesCarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Anotacoes, maxlength: 150 });
    this.SalvarAnotacao = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            salvarAnotacoesClick(e);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var AcompanhamentoEntrega = function (data) {
    let shouldBlink = data.ChamadoEmAberto || (data.Imagem != null && data.Imagem.includes("entrou-e-saiu-sem-entregar")) || data.MotoristaACaminho;

    this.CodigoEntrega = PropertyEntity({ val: data.CodigoEntrega });
    this.Coleta = PropertyEntity({ val: data.Coleta });
    this.DataEntrega = PropertyEntity({ val: data.DataEntregaFormatada });
    this.DataEntregaReprogramada = PropertyEntity({ val: data.DataEntregaReprogramadaFormatada });
    this.DataInicioEntrega = PropertyEntity({ val: data.DataInicioEntregaFormatada });
    this.DataEntregaPrevista = PropertyEntity({ val: data.DataEntregaPrevistaFormatada });
    this.DataFimEntrega = PropertyEntity({ val: data.DataFIMEntregaFormatada });
    this.EntregaNaJanela = PropertyEntity({ val: data.EntregaNaJanela });
    this.LatitudeEntrega = PropertyEntity({ val: data.LatitudeEntrega });
    this.LatitudeFinalizada = PropertyEntity({ val: data.LatitudeFinalizada });
    this.MotoristaACaminho = PropertyEntity({ val: data.MotoristaACaminho });
    this.OrdemPrevista = PropertyEntity({ val: data.OrdemPrevista });
    this.OrdemRealizada = PropertyEntity({ val: data.OrdemRealizada });
    this.Situacao = PropertyEntity({ val: data.Situacao });
    this.Tooltip = PropertyEntity({ val: obterTooltipEntrega(data) });
    this.imagem = PropertyEntity({ val: data.Imagem });
    this.imagemForaRaio = PropertyEntity({ val: data.ImagemForaRaio });
    this.imagemPedidoEmMaisCargas = PropertyEntity({ val: data.ImagemPedidoEmMaisCargas });
    this.imagemForaSequencia = PropertyEntity({ val: data.ImagemForaSequencia });
    this.imagemNotaCobertura = PropertyEntity({ val: data.ImagemNotaCobertura });
    this.imagemParcial = PropertyEntity({ val: data.ImagemParcial });
    this.imagemSemCoordenada = PropertyEntity({ val: data.ImagemSemCoordenada });
    this.imagemPedidoReentrega = PropertyEntity({ val: data.ImagemPedidoReentrega });
    this.imagemAtrasado = PropertyEntity({ val: data.ImagemAtrasado });
    this.imagemTendenciaAtraso = PropertyEntity({ val: data.imagemTendenciaAtraso });
    this.imagemTendenciaAdiantamento = PropertyEntity({ val: data.imagemTendenciaAdiantamento });
    this.imagemReentregarMesmaCarga = PropertyEntity({ val: data.ImagemReentregarMesmaCarga });
    this.Class = PropertyEntity({ val: shouldBlink ? "img-detalhe blink" : "img-detalhe" });
    this.DestacarFiltro = PropertyEntity({ val: data.DestacarFiltro });

    var cliente = ObjetoCliente({
        CPF_CNPJ: data.Cliente[0].CLI_CGCCPF,
        Nome: data.Cliente[0].Nome,
        Endereco: data.Cliente[0].Endereco,
        CEP: data.Cliente[0].CEP,
        Numero: data.Cliente[0].Numero,
        Latitude: data.Cliente[0].Latitude,
        Longitude: data.Cliente[0].Longitude,
    });

    this.Cliente = PropertyEntity({ val: cliente });

    this.DadosEntrega = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirDetalhesEntregaClick(data);

        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

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

//*******FUNCOES PRIVADAS*******

function desactiveBotoesAtendimento() {
    $("#view-pendentes").removeClass("active");
    $("#view-emtratativa").removeClass("active");
    $("#view-atrasadas").removeClass("active");
    $("#view-concluidas").removeClass("active");
}

function fecharDetalhes() {
    $("#DivDetalhesEntrega").fadeOut("fast");
    $("#card_" + _cardAtivo).removeClass("active-card");
    $(".legenda-mapaCards").hide();
    $("#card_" + _cardAtivo).css("box-shadow", "0 3px 10px #ccc");
    _cardAtivo = 0;

    if (_polilinhaPlanejada != null || _polilinhaRealizada != null) {
        try {
            _mapaAcompanhamento.removeLayer(_polilinhaPlanejada);
            _mapaAcompanhamento.removeLayer(_polilinhaRealizada);
        }
        catch (e) { }
    }

    _polilinhaPlanejada = null;
    _polilinhaRealizada = null;

    if (_markersEntregas && _markersEntregas.length > 0) {
        try {
            for (var i = 0; i < _markersEntregas.length; i++) {
                if (_markersEntregas[i] != null) {
                    _mapaAcompanhamento.removeLayer(_markersEntregas[i]);
                }
            }

            _markersEntregas = new Array();
        }
        catch (e) { }
    }

    if (_markerVeiculo != null)
        _markerVeiculo.setIcon(new iconeMarkerCarga(false, _corAnteriorSelecionada));

    $(".legenda-rotas-container").hide();
}

function ObjetoCliente(obj) {
    return $.extend(true, {
        CPF_CNPJ: "",
        backEventClick: function () { },
        Endereco: "",
        CEP: "",
        Numero: "",
        Nome: "",
        Latitude: "",
        Longitude: "",
        NotaFiscal: [],
        Pedido: []
    }, obj);
}

function ObjetoEtapa(obj) {
    return $.extend(true, {
        Carga: 0
    }, obj);
}

function initCarouselIndicatorsWithArrows() {
    var $carousel = $('#carousel-filters');
    var $prevWrap = $('#indicator-prev');
    var $nextWrap = $('#indicator-next');
    if (!$carousel.length || !$prevWrap.length || !$nextWrap.length) return;

    var $prevBtn = $prevWrap.find('[data-bs-slide="prev"]');
    var $nextBtn = $nextWrap.find('[data-bs-slide="next"]');

    function refresh() {
        var inst = bootstrap.Carousel.getInstance($carousel[0]) || bootstrap.Carousel.getOrCreateInstance($carousel[0]);
        var wrapEnabled = inst && inst._config && inst._config.wrap !== false; // true = loop

        var $items = $carousel.find('.carousel-item');
        var idx = $items.index($items.filter('.active'));

        // limpa estados
        $prevWrap.removeClass('is-disabled');
        $nextWrap.removeClass('is-disabled');
        $prevBtn.prop('disabled', false);
        $nextBtn.prop('disabled', false);

        // se wrap estiver DESLIGADO, aplicamos os estados de borda
        if (!wrapEnabled) {
            if (idx <= 0) {
                $prevWrap.addClass('is-disabled');
                $prevBtn.prop('disabled', true);
            }
            if (idx >= $items.length - 1) {
                $nextWrap.addClass('is-disabled');
                $nextBtn.prop('disabled', true);
            }
        }
    }

    $carousel.off('slid.bs.carousel.__indArrows').on('slid.bs.carousel.__indArrows', refresh);
    refresh();
}

function fluxoEntrega() {
    this.DataFimViagem = PropertyEntity({ val: ko.observable(_detalhesCarga.DataFimViagemFormatada.val()) });
    this.Carga = PropertyEntity({ val: ko.observable(_detalhesCarga.CodigoCarga.val()) });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(_detalhesCarga.Veiculo.val()) });
    this.IDEquipamento = PropertyEntity({ val: ko.observable(0) });
    this.DataInicioViagem = PropertyEntity({ val: ko.observable(_detalhesCarga.DataInicioViagemFormatada.val()) });
    this.InicioViagemForaRaio = PropertyEntity({ val: ko.observable(_detalhesCarga.InicioViagemForaRaio.val()) });
    this.ImagemForaRaio = PropertyEntity({ val: ko.observable(_detalhesCarga.ImagemForaRaio.val()) });
    this.DataPreViagemInicio = PropertyEntity({ val: ko.observable(_detalhesCarga.DataPreViagemInicioFormatada.val()) });
    this.DataPreViagemFim = PropertyEntity({ val: ko.observable(_detalhesCarga.DataPreViagemFimFormatada.val()) });
    this.KnoutEtapas = PropertyEntity({ def: [], val: ko.observableArray([]) });
    this.GridMonitoramento = PropertyEntity({ html: ko.observable("..."), def: [], val: ko.observable([]), id: guid(), visible: ko.observable(false) });
    this.NumeroMotorista = PropertyEntity({ val: ko.observable(_detalhesCarga.NumeroMotorista.val()) });
    this.NomeMotorista = PropertyEntity({ val: ko.observable(_detalhesCarga.Motoristas.val()) });
    this.PermiteAdicionarPromotor = PropertyEntity({ val: ko.observable(false) });
}

function obterTooltipEntrega(entrega) {
    descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.Cliente.getFieldDescription() + " " + entrega.Cliente[0].Nome;

    if (entrega.DataEntregaFormatada != "") {
        descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.DataRealizada.getFieldDescription() + " " + entrega.DataEntregaFormatada;
        descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.SequenciaPrevistaSequenciaRealizada.getFieldDescription() + " " + (entrega.OrdemPrevista + 1) + "/" + (entrega.OrdemRealizada + 1); //Sempre soma 1, pois no banco fica 0 para primeria.
    }
    else if (entrega.DataInicioEntregaFormatada != "")
        descricaoTooltip = descricaoTooltip + "<br>" + Localization.Resources.Cargas.ControleEntrega.DataRealizada.getFieldDescription() + " " + entrega.DataInicioEntregaFormatada;

    else {
        if (entrega.DataEntregaPrevistaFormatada != "")
            descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaEntrega.getFieldDescription() + " " + entrega.DataEntregaPrevistaFormatada;
        if (entrega.DataEntregaReprogramadaFormatada != "" && entrega.DataEntregaReprogramada != entrega.DataEntregaPrevista)
            descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.DataDaEntregaReprogramada.getFieldDescription() + " " + entrega.DataEntregaReprogramadaFormatada;
    }

    return descricaoTooltip;
}

function LoadAcompanhamentoCarga() {
    _mostrandoCards = true;
    _map = null;
    _carregouMapa = false;

    _pesquisaAcompanhamentoCarga = new PesquisaAcompanhamentoCarga();
    KoBindings(_pesquisaAcompanhamentoCarga, "knockoutPesquisaAcompanhamentoCarga", false, _pesquisaAcompanhamentoCarga.Pesquisar.id);

    _cardAcompanhamentoCarga = new AcompanhamentoCarga();
    KoBindings(_cardAcompanhamentoCarga, "knoutContainerCardCargas");

    _cabecalhoAcompanhamentoCarga = new ResumoAcompanhamentoCarga();
    KoBindings(_cabecalhoAcompanhamentoCarga, "knoutCabecalhoAcompanhamentoCarga");

    _detalhesCarga = new CardCarga();
    KoBindings(_detalhesCarga, "knoutDetalhesCarga");

    _raioXCarga = new Paradas();
    KoBindings(_raioXCarga, "knoutParadas");

    _detalhesEntregas = new CardCarga();
    KoBindings(_detalhesEntregas, "knoutDetalhesEntrega");

    _modalAnotacoesCarga = new ModalAnotacoesCarga();
    KoBindings(_modalAnotacoesCarga, "knockoutAnotacoesCarga");

    BuscarTransportadores(_pesquisaAcompanhamentoCarga.Transportador);
    BuscarTiposOperacao(_pesquisaAcompanhamentoCarga.TipoOperacao);
    BuscarVeiculos(_pesquisaAcompanhamentoCarga.Veiculos);
    BuscarXMLNotaFiscal(_pesquisaAcompanhamentoCarga.NumerosNotasFiscais);
    BuscarClientes(_pesquisaAcompanhamentoCarga.LocalEntrega);
    BuscarClientes(_pesquisaAcompanhamentoCarga.LocalColeta);
    BuscarClientes(_pesquisaAcompanhamentoCarga.DestinatarioPedido);
    BuscarClientes(_pesquisaAcompanhamentoCarga.RemetentePedido);
    BuscarClienteComplementar(_pesquisaAcompanhamentoCarga.ClientesComplementar);
    BuscarFilial(_pesquisaAcompanhamentoCarga.Filial);
    BuscarLocalidades(_pesquisaAcompanhamentoCarga.CidadeColeta);
    BuscarLocalidades(_pesquisaAcompanhamentoCarga.CidadeEntrega);
    BuscarMotoristas(_pesquisaAcompanhamentoCarga.Motorista);
    BuscarClientes(_pesquisaAcompanhamentoCarga.Expedidor);
    BuscarClientes(_pesquisaAcompanhamentoCarga.Recebedor);
    BuscarFilial(_pesquisaAcompanhamentoCarga.FilialVenda);
    BuscarTiposdeCarga(_pesquisaAcompanhamentoCarga.TipoCarga);
    BuscarTipoDocumentoTransporte(_pesquisaAcompanhamentoCarga.TipoDocumentoTransporte);
    BuscarCanaisVenda(_pesquisaAcompanhamentoCarga.CanalVenda);
    BuscarEventosMonitoramento(_pesquisaAcompanhamentoCarga.TipoAlerta);
    BuscarMesoRegiao(_pesquisaAcompanhamentoCarga.Mesoregiao);
    BuscarRegioes(_pesquisaAcompanhamentoCarga.Regiao);
    BuscarGruposPessoas(_pesquisaAcompanhamentoCarga.GrupoDePessoas);
    BuscarCanaisEntrega(_pesquisaAcompanhamentoCarga.CanalEntrega);

    LoadSignalRAcompanhamentoCarga();
    LoadPedidosOutrasCargas();
    LoadConfiguracaoWidgets();

    BuscarStatusViagem(function () {
        buscarDetalhesOperador(function () {
            carregarHTMLComponenteControleEntrega(function () {
                registraComponente(); //função dos scripts antigos
                loadEtapasControleEntrega(); //função dos scripts antigos
                loadChamado(); //função dos scripts antigos
                loadAdicionarPedidoEntrega(); //função dos scripts antigos
                loadDetalhesCargaEspelhada();
            });
        });

        LoadFiltroPesquisaAcompanhamentoCarga().then(function () {
            ObterResumoAcompanhamentoCargas();

            BuscarCargasAcompanhamento(1, false).then(function () {
                AplicarConfigWidget();
            });

            loadCRUDTratativaAlerta();
            loadDetalhesMonitoramento();
            loadHistoricoMonitoramento();
        });
    }, _pesquisaAcompanhamentoCarga.StatusDaViagem)
    $(document).ready(function () {
        $('[data-bs-toggle="tooltip"]').tooltip();
        initCarouselIndicatorsWithArrows();
    });
}

function BuscarStatusViagem(callback, statusViagem) {
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

function BuscarCargasAcompanhamento(page, eventoPorPaginacao) {
    var p = new promise.Promise();
    var itensPorPagina = 50;

    var data = RetornarObjetoPesquisa(_pesquisaAcompanhamentoCarga);
    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;

    data.resumoTodas = false;
    data.resumoEmViagem = false;
    data.resumoNaoIniciada = false;
    data.resumoFinalizadas = false;

    if (_FiltroResumoViagemSelecionado == EnumFiltroViagensAcompanhamentoCarga.Todas)
        data.resumoTodas = true;
    else if (_FiltroResumoViagemSelecionado == EnumFiltroViagensAcompanhamentoCarga.EmViagem)
        data.resumoEmViagem = true;
    else if (_FiltroResumoViagemSelecionado == EnumFiltroViagensAcompanhamentoCarga.NaoIniciada)
        data.resumoNaoIniciada = true;
    else if (_FiltroResumoViagemSelecionado == EnumFiltroViagensAcompanhamentoCarga.Finalizadas)
        data.resumoFinalizadas = true;

    data.FiltroAtendimento = _FiltroResumoAtendimentoSelecionado;
    data.FiltroTendencia = _FiltroTendenciaPrazoEntrega;
    data.FiltroEspelhamento = _FiltroEspelhamentoSelecionado;

    _posicaoPin = 0;

    executarReST("AcompanhamentoCarga/ObterCargasAcompanhamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                $('[rel=popover-hover]').each(function () {
                    let popover = bootstrap.Popover.getOrCreateInstance(this);
                    popover.dispose();
                });

                _cardAcompanhamentoCarga.Cargas.removeAll();

                if (arg.Data.Cargas != undefined) {
                    for (var i = 0; i < arg.Data.Cargas.length; i++) {
                        var data = arg.Data.Cargas[i];
                        if (data.CargaFixadaControleCargas)
                            _posicaoPin += 1;

                        var card = new CardCarga(data);
                        _cardAcompanhamentoCarga.Cargas.push(card);
                        controlarVisibilidadeTratativaAlertaLoadCardCarga(card);
                    }
                }

                configurarPaginacao(page, eventoPorPaginacao, arg, itensPorPagina);
                window.scrollTo(0, 0);

                Global.fecharModal("knockoutPesquisaAcompanhamentoCarga");

                setTimeout(function () {
                    $("[rel=popover-hover]").each(function () {
                        bootstrap.Popover.getOrCreateInstance(this, { placement: "top", html: true, trigger: "hover", container: "body", delay: { "show": 500, "hide": 100 } });
                    });
                }, 500);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        p.done();
    });

    return p;
}

function ObterResumoAcompanhamentoCargas() {
    var data = RetornarObjetoPesquisa(_pesquisaAcompanhamentoCarga);
    data.FiltroTendencia = _FiltroTendenciaPrazoEntrega;

    executarReST("AcompanhamentoCarga/ObterResumoCargasAcompanhamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                $('[rel=popover-hover]').each(function () {
                    let popover = bootstrap.Popover.getOrCreateInstance(this);
                    popover.dispose();
                });


                if (arg.Data.TotalCargas != undefined) {
                    _cabecalhoAcompanhamentoCarga.resumoTodas.val(arg.Data.TotalCargas);
                    _cabecalhoAcompanhamentoCarga.resumoNaoIniciada.val(arg.Data.TotalCargasNaoIniciadas);
                    _cabecalhoAcompanhamentoCarga.resumoEmViagem.val(arg.Data.TotalCargasEmViagem);
                    _cabecalhoAcompanhamentoCarga.resumoFinalizadas.val(arg.Data.TotalCargasFinalizadas);

                    _cabecalhoAcompanhamentoCarga.atendimentosPendentes.val(arg.Data.TotaisChamadosPendentes);
                    _cabecalhoAcompanhamentoCarga.atendimentosTratativas.val(arg.Data.TotaisChamadosTratativa);
                    _cabecalhoAcompanhamentoCarga.atendimentosAtrasados.val(arg.Data.TotaisChamadoAtrasados);
                    _cabecalhoAcompanhamentoCarga.atendimentosConcluidos.val(arg.Data.TotaisChamadosConcluidos);
                    _cabecalhoAcompanhamentoCarga.alertasEmAberto.val(arg.Data.TotalCargasComAlertas);
                    _cabecalhoAcompanhamentoCarga.alertasEmTratativa.val(arg.Data.TotalCargasComAlertasEmTratativa);
                    _cabecalhoAcompanhamentoCarga.alertasFinalizados.val(arg.Data.TotalCargasSemAlertas);
                    _cabecalhoAcompanhamentoCarga.tendenciaEntregaNenhum.val(arg.Data.TotalTendenciaEntregaNenhum);
                    _cabecalhoAcompanhamentoCarga.tendenciaEntregaAdiantado.val(arg.Data.TotalTendenciaEntregaAdiantado);
                    _cabecalhoAcompanhamentoCarga.tendenciaEntregaNoPrazo.val(arg.Data.TotalTendenciaEntregaNoPrazo);
                    _cabecalhoAcompanhamentoCarga.tendenciaEntregaAtraso.val(arg.Data.TotalTendenciaEntregaAtraso);
                    _cabecalhoAcompanhamentoCarga.tendenciaEntregaAtrasado.val(arg.Data.TotalTendenciaEntregaAtrasado);
                    _cabecalhoAcompanhamentoCarga.quantidadeFarolEspelhamentoOnline.val(arg.Data.TotalFarolEspelhamentoOnline);
                    _cabecalhoAcompanhamentoCarga.quantidadeFarolEspelhamentoOffline.val(arg.Data.TotalFarolEspelhamentoOffline);

                    const onlineCount = _cabecalhoAcompanhamentoCarga.quantidadeFarolEspelhamentoOnline.val();
                    const offlineCount = _cabecalhoAcompanhamentoCarga.quantidadeFarolEspelhamentoOffline.val();

                    const total = onlineCount + offlineCount;

                    const onlinePct = total > 0 ? Math.round((onlineCount / total) * 100) : (onlineCount > 0 ? 100 : 0);
                    const offlinePct = total > 0 ? Math.round((offlineCount / total) * 100) : (offlineCount > 0 ? 100 : 0);

                    _cabecalhoAcompanhamentoCarga.quantidadeFarolEspelhamentoOnline.porcentagem(`Online - ${onlinePct}%`);
                    _cabecalhoAcompanhamentoCarga.quantidadeFarolEspelhamentoOffline.porcentagem(`Offline - ${offlinePct}%`);

                    
                    $("#divResumos").show();
                    initCarouselIndicatorsWithArrows();
                }

                setTimeout(function () {
                    $("[rel=popover-hover]").each(function () {
                        bootstrap.Popover.getOrCreateInstance(this, { placement: "top", html: true, trigger: "hover", container: "body", delay: { "show": 500, "hide": 100 } });
                    });
                }, 500);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ObterMensagensNaoLidasCards() {
    var data = RetornarObjetoPesquisa(_pesquisaAcompanhamentoCarga);
    executarReST("AcompanhamentoCarga/ObterMensagensNaoLidasCards", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (_cardAcompanhamentoCarga.Cargas() != undefined) {
                    for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {
                        for (var j = 0; j < arg.Data.Mensagens.length; j++) {
                            var mensagem = arg.Data.Mensagens[j];
                            if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == mensagem.CodigoCarga) {
                                _cardAcompanhamentoCarga.Cargas()[i].PossuiMensagemNaoLida.val(true);
                                break;
                            } else {
                                _cardAcompanhamentoCarga.Cargas()[i].PossuiMensagemNaoLida.val(false);
                            }
                        }
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, false);

}

function configurarPaginacao(page, paginou, arg, itensPorPagina) {
    var clicouNoPaginar = false;

    if (!paginou) {
        if (arg.QuantidadeRegistros > 0) {

            var paginas = Math.ceil((arg.QuantidadeRegistros / itensPorPagina));

            if (paginas > 1) {
                $("#divPaginacaoCards").html('<ul style="float:right" id="paginacaoCargas" class="pagination"></ul>');
                $('#paginacaoCargas').twbsPagination({
                    first: Localization.Resources.Cargas.ControleEntrega.Primeiro,
                    prev: Localization.Resources.Cargas.ControleEntrega.Anterior,
                    next: Localization.Resources.Cargas.ControleEntrega.Proximo,
                    last: Localization.Resources.Cargas.ControleEntrega.Ultimo,
                    totalPages: paginas,
                    visiblePages: 5,
                    onPageClick: function (event, page) {
                        if (clicouNoPaginar)
                            BuscarCargasAcompanhamento(page, true).then(function () {
                                AplicarConfigWidget();
                            });


                        clicouNoPaginar = true;
                    }
                });
            } else
                $("#divPaginacaoCards").html('');

        }
        else
            $("#divPaginacaoCards").html(Localization.Resources.Cargas.ControleEntrega.NehumRegistroEncontrado);
    }
}

function controlarExibicoes(exibirCards) {
    _mostrandoCards = exibirCards;

    if (!_mostrandoCards) {
        $("#btnCards").addClass("button-cards");

        $("#knoutContainerCardCargas").hide();
        $("#knoutMapa").show();

        if (!_carregouMapa) {
            loadMapa();
            loadCargasNoMapa();
        }

        $(".legenda-mapaVeiculos").show();
    } else {

        $("#btnMap").addClass("button-map");

        $("#knoutContainerCardCargas").show();
        $("#knoutMapa").hide();

        $(".legenda-mapaVeiculos").hide();
    }
}

function assumirMonitoramentoCarga(data) {
    exibirModalDelegarMonitoramento(true, _detalhesCarga.CodigoCarga.val());
}

function exibirModalAnotacoesCarga() {
    _modalAnotacoesCarga.CodigoCarga.val(_detalhesCarga.CodigoCarga.val());
    Global.abrirModal("divModalAnotacoesCarga");
}

//*******EVENTOS*******

function atribuirAnotacoesCargaClick() {
    executarReST("AcompanhamentoCarga/ObterAnotacoesCarga", { carga: _detalhesCarga.CodigoCarga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _modalAnotacoesCarga.AnotacoesCarga.val(arg.Data);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
    exibirModalAnotacoesCarga();
}

function abrirModalOcorrencia() {
    $('#divModalOcorrencia').on('hidden.bs.modal', function () {
    });

    _chamadoOcorrenciaModalOcorrencia = new bootstrap.Modal(document.getElementById("divModalOcorrencia"), { backdrop: 'static' });
    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia", function () {
        _ocorrencia.Carga.codEntity(_detalhesCarga.CodigoCarga.val());
        _ocorrencia.Carga.val(_detalhesCarga.CargaEmbarcador.val());
        retornoCarga({ Codigo: _detalhesCarga.CodigoCarga.val(), CodigoCargaEmbarcador: _detalhesCarga.CargaEmbarcador.val() });
    });

    Global.abrirModal('divModalOcorrencia');
}

function salvarAnotacoesClick() {
    Salvar(_modalAnotacoesCarga, "AcompanhamentoCarga/SalvarAnotacoesCarga", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "As anotações foram salvas.");
                LimparCampos(_modalAnotacoesCarga);
                Global.fecharModal("divModalAnotacoesCarga");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function viewDetalhesCargaClick(knokoutCardCarga) {

    $('[rel=popover-hover]').each(function () {
        let popover = bootstrap.Popover.getOrCreateInstance(this);
        popover.dispose();
    });

    if (knokoutCardCarga.Data != undefined) {
        PreencherObjetoKnout(_detalhesCarga, knokoutCardCarga);
        PreencherObjetoKnout(_detalhesEntregas, knokoutCardCarga);

        _detalhesEntregas.Entregas.val.removeAll();

        for (var i = 0; i < knokoutCardCarga.Data.Entregas.length; i++) {
            var entrega = knokoutCardCarga.Data.Entregas[i];
            var EntregaKnockout = new AcompanhamentoEntrega(entrega);
            _detalhesEntregas.Entregas.val.push(EntregaKnockout);
        }
    }

    setTimeout(function () {
        $("[rel=popover-hover]").each(function () {
            bootstrap.Popover.getOrCreateInstance(this, { placement: "top", html: true, trigger: "hover", container: "body", delay: { "show": 500, "hide": 100 } });
        });
    }, 500);

    if (_cardAtivo != _detalhesCarga.CodigoCarga.val()) {
        $("#card_" + _cardAtivo).removeClass("active-card");
        $("#card_" + _cardAtivo).css("box-shadow", "0 3px 10px #ccc");

        _cardAtivo = _detalhesCarga.CodigoCarga.val();
    }

    $("#card_" + _detalhesCarga.CodigoCarga.val()).addClass("active-card");
    $("#card_" + _cardAtivo).css("box-shadow", "0 3px 10px #3E3F40");

    $("#DivDetalhesEntrega").fadeIn("fast")
}

function adicionarEventoCarga() {
    _etapaAtualFluxo = null;

    if (_etapaAtualFluxo == undefined || _etapaAtualFluxo == null)
        _etapaAtualFluxo = new fluxoEntrega();

    adicionarEventosClick() //função dos scripts antigos
}

function abrirRaioXCarga(data) {
    _raioXCarga.Carga.val(data.CodigoCarga);
    executarReST("Carga/BuscarRaioXCarga", { Codigo: data.CodigoCarga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                loadGridCargaParadas(data);
                Global.abrirModal('divModalRaioXDaCarga');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

function loadGridCargaParadas(data) {
    var configuracoesExportacao = { url: "ControleEntregaRaiox/ExportarPesquisaParadas", titulo: Localization.Resources.Cargas.ControleEntrega.Paradas };

    var ordenacao = {
        column: 7,
        dir: orderDir.asc,
    };

    _raioXCarga.Carga.val(data.CodigoCarga.val());
    _gridParadas = new GridViewExportacao(_raioXCarga.Paradas.idGrid, "ControleEntregaRaiox/PesquisaParadas", _raioXCarga, null, configuracoesExportacao, ordenacao, 1000);
    _gridParadas.CarregarGrid();

}

function pedidoReentregaClick() {
    _etapaAtualFluxo = null;

    if (_etapaAtualFluxo == undefined || _etapaAtualFluxo == null)
        _etapaAtualFluxo = new fluxoEntrega();

    adicionarPedidoReentregaClick() //função dos scripts antigos
}

function exibirDadosCargaClick() {
    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();

    visualizarDadosCargaClick(); //função dos scripts antigos
}

function exibirModalDetalhesPedidoClick(carga) {
    loadDetalhesPedidosTorreControle(carga.CodigoCarga.val());
}

function downloadRelatorioBoletimViagem() {
    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();

    gerarRelatorioBoletimViagem(); //função dos scripts antigos
}

function exibirInicioViagemClick(data) {
    var etapa = ObjetoEtapa({
        Carga: data.CodigoCarga.val()
    });

    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();
    _etapaAtualFluxo.Carga.val(data.CodigoCarga.val());

    exibirDetalhesInicioViagemControleEntrega(fluxoEntrega, etapa); //função dos scripts antigos
}

function exibirFimViagemClick(data) {
    var etapa = ObjetoEtapa({
        Carga: data.CodigoCarga.val()
    });

    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();
    _etapaAtualFluxo.Carga.val(data.CodigoCarga.val());

    exibirFimViagem(fluxoEntrega, etapa); //função dos scripts antigos
}

function exibirInicioPreTripClick(data) {
    var etapa = ObjetoEtapa({
        Carga: data.CodigoCarga.val()
    });

    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();
    _etapaAtualFluxo.Carga.val(data.CodigoCarga.val());

    informarInicioPreTrip(_etapaAtualFluxo, etapa);


}

function exibirFimPreTripClick(data) {
    var etapa = ObjetoEtapa({
        Carga: data.CodigoCarga.val()
    });

    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();
    _etapaAtualFluxo.Carga.val(data.CodigoCarga.val());

    informarFimPreTrip(_etapaAtualFluxo, etapa);

}

function dadosMapaClick() {
    controlarExibicoes(!_mostrandoCards);
    $(".legenda-mapaCards").hide();
    _detalhesCarga.VisualizarNoMapa.text(_mostrandoCards ? "Visualizar no Mapa" : "Visualizar no Card")
    if (_mostrandoCards)
        return;
    let v = _cardAcompanhamentoCarga.Cargas().find(x => x.CodigoCarga.val() == _detalhesCarga.CodigoCarga.val());
    let marker = L.marker([_detalhesCarga.Latitude.val(), _detalhesCarga.Longitude.val()], { icon: new iconeMarkerCarga(true, v.Cor.val()) });
    clickMarkerVeiculoCard(v, marker);
    loadCargasNoMapa();
    //_etapaAtualFluxo = null;
    //_etapaAtualFluxo = new fluxoEntrega();

    //visualizarDadosMapaClick(); //função dos scripts antigos
}

function abrirChatMensagemClick(data) {
    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();

    _etapaAtualFluxo.Carga.val(data.CodigoCarga.val());
    _etapaAtualFluxo.NomeMotorista.val(data.Motoristas.val());
    _etapaAtualFluxo.NumeroMotorista.val(data.NumeroMotorista.val());
    _etapaAtualFluxo.PermiteAdicionarPromotor.val(false);

    abrirChatClick(); //função dos scripts antigos
}

function exibirDetalhesEntregaClick(entrega) {
    _etapaAtualFluxo = null;
    _etapaAtualFluxo = new fluxoEntrega();

    //_etapaAtualFluxo.DataFimViagem.val(_detalhesCarga.DataFimViagemFormatada.val());
    if (_entrega == null) {
        _entrega = new EntregaPedido();
        KoBindings(_entrega, "knockoutEntrega");
    }

    limparCamposDetalhesEntrega();

    var path = document.location.href;
    var page = path.split("/").pop();

    executarReST("ControleEntregaEntrega/BuscarDetalhesEntrega", { Codigo: entrega.CodigoEntrega, Page: page }, function (arg) {
        if (arg.Success) {
            var data = arg.Data;
            if (data !== false) {
                PreencherDetalhesEntrega(data); //função dos scripts antigos
                _dadosDetalhesEntrega = data;
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExibirLegendaClick() {
    Global.abrirModal('divModalLegendaAcompanhamentoCarga');
}

function fixarPinCardClick(ui) {
    var fixar = false;
    var codigo = ui.CodigoCarga.val();
    var classe = $("#pin_" + codigo).attr('class');

    if (classe.toLowerCase().indexOf("pin-active") >= 0) {
        fixar = false;
    } else {
        fixar = true;
    }

    executarReST("AcompanhamentoCarga/FixarCargaPin", { Codigo: codigo, Fixar: fixar }, function (arg) {
        if (arg.Success) {
            var data = arg.Data;
            if (data !== false) {

                if (fixar)
                    $("#pin_" + codigo).addClass('pin-active');
                else
                    $("#pin_" + codigo).removeClass('pin-active');

                _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.Todas;
                _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;

                BuscarCargasAcompanhamento(1, false).then(function () {
                    AplicarConfigWidget();
                });

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function removerTagNovoCard(ui) {
    var codigo = ui.CodigoCarga.val();
    for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {
        if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == codigo) {
            _cardAcompanhamentoCarga.Cargas()[i].NovoCard.val(false);
            break;
        }
    }
}

function fecharFiltrosBusca() {
    Global.fecharModal("knockoutPesquisaAcompanhamentoCarga");
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

function visualizarHistoricosClick() {
    exibirHistoricoMonitoramentoPorCodigo(_detalhesCarga.CodigoMonitoramento.val());
}

function atualizaTituloModalCarga(row) {
    $(".title-carga-codigo-embarcador").html(row.CargaEmbarcador);
    $(".title-carga-placa").html(row.Tracao + " " + row.Reboques);
}

function visualizarDetalhesMonitoramentoClick() {
    exibirDetalhesMonitoramentoPorCodigo(_detalhesCarga.CodigoMonitoramento.val());
}

function visualizarDetalhesTorreClick() {
    loadModalDetalhesTorre(_detalhesCarga.CodigoCarga.val());
}

function LoadFiltroPesquisaAcompanhamentoCarga() {
    return new Promise((resolve) => {
        var data = { TipoFiltro: EnumCodigoFiltroPesquisa.AcompanhamentoCarga };

        executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
            if (res.Success && Boolean(res.Data)) {
                PreencherJsonFiltroPesquisa(_pesquisaAcompanhamentoCarga, res.Data.Dados);
                _pesquisaAcompanhamentoCarga.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
                _pesquisaAcompanhamentoCarga.ModeloFiltrosPesquisa.val(res.Data.Descricao);

                if (_pesquisaAcompanhamentoCarga.ModeloFiltrosPesquisa.callbackRetornoPesquisa instanceof Function)
                    _pesquisaAcompanhamentoCarga.ModeloFiltrosPesquisa.callbackRetornoPesquisa();
            }

            if (_pesquisaAcompanhamentoCarga.DataCriacaoCargaInicial.val() === "" || _pesquisaAcompanhamentoCarga.DataCriacaoCargaFinal.val() === "") {
                _pesquisaAcompanhamentoCarga.DataCriacaoCargaInicial.val(Global.Data(EnumTipoOperacaoDate.Subtract, 2, EnumTipoOperacaoObjetoDate.Days));
                _pesquisaAcompanhamentoCarga.DataCriacaoCargaFinal.val(Global.Data(EnumTipoOperacaoDate.Add, 2, EnumTipoOperacaoObjetoDate.Days));
            }

            resolve();
        });
    });
}

function ControlarVisibilidadeBotoesAcessoRapido(botaoPrimario, botaoSecundario) {

    _configWidgetAcompanhamentoCarga.BotaoPrimario.val(botaoPrimario);
    _configWidgetAcompanhamentoCarga.BotaoSecundario.val(botaoSecundario);

    _detalhesCarga.AdicionarPedidoReentrega.visibleVisualizacaoRapida(false);
    _detalhesCarga.VisualizarDadosCarga.visibleVisualizacaoRapida(false);
    _detalhesCarga.VisualizarNoMapa.visibleVisualizacaoRapida(false);
    _detalhesCarga.DownloadBoletimEmbarque.visibleVisualizacaoRapida(false);
    _detalhesCarga.AdicionarEvento.visibleVisualizacaoRapida(false);
    _detalhesCarga.RaioXCarga.visibleVisualizacaoRapida(false);
    _detalhesCarga.Assumir.visibleVisualizacaoRapida(false);
    _detalhesCarga.Anotacoes.visibleVisualizacaoRapida(false);
    _detalhesCarga.OcorrenciaFrete.visibleVisualizacaoRapida(false);
    _detalhesCarga.VisualizarHistoricoMonitoramento.visibleVisualizacaoRapida(false);

    switch (botaoPrimario) {
        case 1:
            _detalhesCarga.AdicionarPedidoReentrega.visibleVisualizacaoRapida(true)
            $('.pedido-reentrega').addClass('botao-primario');
            break;
        case 2:
            _detalhesCarga.VisualizarDadosCarga.visibleVisualizacaoRapida(true)
            $('.dados-carga').addClass('botao-primario');
            break;
        case 3:
            _detalhesCarga.VisualizarNoMapa.visibleVisualizacaoRapida(true)
            $('.visualizar-mapa').addClass('botao-primario');
            break;
        case 4:
            _detalhesCarga.DownloadBoletimEmbarque.visibleVisualizacaoRapida(true)
            $('.boletim-embarque').addClass('botao-primario');
            break;
        case 5:
            _detalhesCarga.AdicionarEvento.visibleVisualizacaoRapida(true)
            $('.adicionar-evento').addClass('botao-primario');
            break;
        case 6:
            _detalhesCarga.RaioXCarga.visibleVisualizacaoRapida(true)
            $('.raio-x-carga').addClass('botao-primario');
            break;
        case 7:
            _detalhesCarga.Assumir.visibleVisualizacaoRapida(true)
            $('.assumir').addClass('botao-primario');
            break;
        case 8:
            _detalhesCarga.Anotacoes.visibleVisualizacaoRapida(true)
            $('.anotacoes').addClass('botao-primario');
            break;
        case 9:
            _detalhesCarga.OcorrenciaFrete.visibleVisualizacaoRapida(true)
            break;
        case 10:
            _detalhesCarga.VisualizarHistoricoMonitoramento.visibleVisualizacaoRapida(true)
            $('.historico').addClass('botao-primario');
            break;
        case 11:
            _detalhesCarga.VisualizarDetalhesPedidos.visibleVisualizacaoRapida(true)
            $('.detalhes-pedidos').addClass('botao-primario');
            break;
    }

    switch (botaoSecundario) {
        case 1:
            _detalhesCarga.AdicionarPedidoReentrega.visibleVisualizacaoRapida(true)
            $('.pedido-reentrega').addClass('botao-secundario');
            break;
        case 2:
            _detalhesCarga.VisualizarDadosCarga.visibleVisualizacaoRapida(true)
            $('.dados-carga').addClass('botao-secundario');
            break;
        case 3:
            _detalhesCarga.VisualizarNoMapa.visibleVisualizacaoRapida(true)
            $('.visualizar-mapa').addClass('botao-secundario');
            break;
        case 4:
            _detalhesCarga.DownloadBoletimEmbarque.visibleVisualizacaoRapida(true)
            $('.boletim-embarque').addClass('botao-secundario');
            break;
        case 5:
            _detalhesCarga.AdicionarEvento.visibleVisualizacaoRapida(true)
            $('.adicionar-evento').addClass('botao-secundario');
            break;
        case 6:
            _detalhesCarga.RaioXCarga.visibleVisualizacaoRapida(true)
            $('.raio-x-carga').addClass('botao-secundario');
            break;
        case 7:
            _detalhesCarga.Assumir.visibleVisualizacaoRapida(true)
            $('.assumir').addClass('botao-secundario');
            break;
        case 8:
            _detalhesCarga.Anotacoes.visibleVisualizacaoRapida(true)
            $('.anotacoes').addClass('botao-secundario');
            break;
        case 9:
            _detalhesCarga.OcorrenciaFrete.visibleVisualizacaoRapida(true)
            break;
        case 10:
            _detalhesCarga.VisualizarHistoricoMonitoramento.visibleVisualizacaoRapida(true)
            $('.historico').addClass('botao-secundario');
            break;
        case 11:
            _detalhesCarga.VisualizarDetalhesPedidos.visibleVisualizacaoRapida(true)
            $('.detalhes-pedidos').addClass('botao-secundario');
            break;
    }
}

//Tratativas de Alerta
function AssumirAlertaCardCargaClick(carga) {
    _cardCargaAlertaTratado = carga;
    AssumirAlerta(carga.CodigoUltimoAlerta.val(), controlarVisibilidadeTratativaAlertaCardCarga);
};
function DeixarAlertaCardCargaClick(carga) {
    _cardCargaAlertaTratado = carga;
    DeixarAlerta(carga.CodigoUltimoAlerta.val(), controlarVisibilidadeTratativaAlertaCardCarga);
};
function ResolverAlertaCardCargaClick(carga) {
    _cardCargaAlertaTratado = carga;
    if (_tratativaAlerta == undefined)
        _tratativaAlerta = new TratativaAlerta();
    ResolverAlerta(carga.CodigoUltimoAlerta.val(), controlarVisibilidadeTratativaAlertaCardCarga);
};
function DetalhesAlertaCardCargaClick(carga) {
    loadTratativaAlerta({ CodigoAlerta: parseInt(carga.CodigoUltimoAlerta.val()) }, []);
};

function controlarVisibilidadeTratativaAlertaCardCarga(data) {
    if (data && data.Status) {
        _cardCargaAlertaTratado.NomeResponsavelUltimoAlertaMonitoramento.val("-");
        _cardCargaAlertaTratado.AssumirAlerta.visible(false);
        _cardCargaAlertaTratado.DeixarAlerta.visible(false);
        //_cardCargaAlertaTratado.ResolverAlerta.visible(false);

        if (data.EnumStatus == EnumAlertaMonitorStatus.EmAberto) {
            _cardCargaAlertaTratado.AssumirAlerta.visible(true);
            _cardCargaAlertaTratado.DescricaoStatusUltimoAlertaMonitoramento.val(EnumAlertaMonitorStatus.obterDescricao(data.EnumStatus));
        }
        else if (data.EnumStatus == EnumAlertaMonitorStatus.EmTratativa) {
            _cardCargaAlertaTratado.DeixarAlerta.visible(true);
            //_cardCargaAlertaTratado.ResolverAlerta.visible(true);
            _cardCargaAlertaTratado.NomeResponsavelUltimoAlertaMonitoramento.val(data.Responsavel);
            _cardCargaAlertaTratado.DescricaoStatusUltimoAlertaMonitoramento.val(EnumAlertaMonitorStatus.obterDescricao(data.EnumStatus));
        }
    }
}

function controlarVisibilidadeTratativaAlertaLoadCardCarga(card) {
    let usuarioResponsavel = card.NomeResponsavelUltimoAlertaMonitoramento.val() == _NomeUsuarioLogado;

    card.AssumirAlerta.visible(card.NomeResponsavelUltimoAlertaMonitoramento.val() == '-');
    card.DeixarAlerta.visible(false);
    //card.ResolverAlerta.visible(false);

    if (card.StatusUltimoAlertaMonitoramento.val() == EnumAlertaMonitorStatus.EmAberto) {
        card.AssumirAlerta.visible(true);
        card.DescricaoResponsavelOUTempoAberto.val(Localization.Resources.Cargas.ControleEntrega.TempoAberto);
    }
    else if (card.StatusUltimoAlertaMonitoramento.val() == EnumAlertaMonitorStatus.EmTratativa) {
        card.DescricaoResponsavelOUTempoAberto.val(Localization.Resources.Cargas.ControleEntrega.Responsavel);
        card.DeixarAlerta.visible(usuarioResponsavel);
        //card.ResolverAlerta.visible(usuarioResponsavel);
    }
}
//Tratativas de Alerta

function abrirModalAlterarVeiculoDaCarga(carga) {
    loadModalAlterarVeiculoDaCarga(carga);
}

function loadModalAlterarVeiculoDaCarga(cardCarga) {
    limparModalAlterarVeiculoDaCarga();
    limparMotoristaAlterarVeiculoDaCarga();
    
    _alterarVeiculoAcompanhamentoCarga = new AlterarVeiculoAcompanhamentoCarga(cardCarga);
    KoBindings(_alterarVeiculoAcompanhamentoCarga, "knockoutModalAlterarVeiculoAcompanhamentoCarga");

    var nomeTransportador = cardCarga.NomeTransportador.val() != null ? cardCarga.NomeTransportador.val() : "---";

    _alterarVeiculoAcompanhamentoCarga.CodigoCarga.val(cardCarga.CodigoCarga.val());
    _alterarVeiculoAcompanhamentoCarga.Transportador.val(nomeTransportador);
    _alterarVeiculoAcompanhamentoCarga.SubTitulo.val(_alterarVeiculoAcompanhamentoCarga.SubTitulo.val().replace("{1}", nomeTransportador));
    _alterarVeiculoAcompanhamentoCarga.SubTituloMotorista.val(_alterarVeiculoAcompanhamentoCarga.SubTituloMotorista.val().replace("{1}", nomeTransportador));

    executarReST("AcompanhamentoCarga/ValidarAlteracaoVeiculoAcompanhamentoCarga", { CodigoCarga: cardCarga.CodigoCarga.val() }, function (retorno) {
        if (retorno.Success) {

            if (retorno.Data && retorno.Data.AlteracaoVeiculoPermitida === true) {

                new BuscarModelosVeicularesCarga(_alterarVeiculoAcompanhamentoCarga.ModeloVeiculo);

                new BuscarMotoristas(_alterarVeiculoAcompanhamentoCarga.CodigoMotorista);

                Global.abrirModal("divModalAlterarVeiculoAcompanhamentoCarga");

                $("#etapaAlteracaoVeiculoAlterarVeiculoAcompanhamentoCarga").show();
                $("#etapaAcompanhamentoIntegracoesAlterarVeiculoAcompanhamentoCarga").hide();

                let multiplaEscolha = {
                    basicGrid: null,
                    eventos: {},
                    selecionados: new Array(),
                    naoSelecionados: new Array(),
                    SelecionarTodosKnout: null,
                    callbackNaoSelecionado: null,
                    callbackSelecionado: null,
                    callbackSelecionarTodos: null,
                    somenteLeitura: false,
                    permitirSelecionarSomenteUmRegistro: true
                };

                let ordenacaoPadrao = { column: 2, dir: orderDir.asc };

                _gridAlterarVeiculoAcompanhamentoCarga = new GridView(_alterarVeiculoAcompanhamentoCarga.GridVeiculos.idGrid, "AcompanhamentoCarga/PesquisarVeiculosAlterarVeiculoAcompanhamentoCarga", _alterarVeiculoAcompanhamentoCarga, null, ordenacaoPadrao, 5, null, null, null, multiplaEscolha);

                gridVeiculoAlterarMotoristaAcompanhamentoCarga(false);
               
                $(".alterar-veiculo-alterar-motorista-cpf").mask("000.000.000-00", { selectOnFocus: true, clearIfNotMatch: true });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Data.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function gridVeiculoAlterarMotoristaAcompanhamentoCarga(carregarGrid) {

    _gridAlterarVeiculoAlterarMotoristaAcompanhamentoCarga = null;

    let ordenacaoPadrao = { column: 2, dir: orderDir.asc };

    let multiplaEscolhaMotorista = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: null,
        callbackSelecionado: null,
        callbackSelecionarTodos: null,
        somenteLeitura: false,
        permitirSelecionarSomenteUmRegistro: false
    };

    _gridAlterarVeiculoAlterarMotoristaAcompanhamentoCarga = new GridView(_alterarVeiculoAcompanhamentoCarga.GridMotoristas.idGrid, "AcompanhamentoCarga/PesquisarMotoristasAlterarVeiculoAcompanhamentoCarga", _alterarVeiculoAcompanhamentoCarga, null, ordenacaoPadrao, 5, null, null, null, multiplaEscolhaMotorista);

    if (carregarGrid) {
        _gridAlterarVeiculoAlterarMotoristaAcompanhamentoCarga.CarregarGrid();
    }
}

function fecharModalAlterarVeiculoDaCarga() {
    limparModalAlterarVeiculoDaCarga();
    Global.fecharModal("divModalAlterarVeiculoAcompanhamentoCarga");
}

function limparModalAlterarVeiculoDaCarga() {
    let grid = $("#" + _alterarVeiculoAcompanhamentoCarga?.GridVeiculos?.idGrid + "_wrapper")
    if (grid)
        grid.html('<table width="100%" class="table table-bordered table-hover" data-bind="attr:{id: GridVeiculos.idGrid}" cellspacing="0"></table>');
    _gridAlterarVeiculoAcompanhamentoCarga = null;

    LimparCampos(_alterarVeiculoAcompanhamentoCarga);
}

function limparMotoristaAlterarVeiculoDaCarga() {
    let gridMotoristas = $("#" + _alterarVeiculoAcompanhamentoCarga?.GridMotoristas?.idGrid + "_wrapper")
    if (gridMotoristas)
        gridMotoristas.html('<table width="100%" class="table table-bordered table-hover" data-bind="attr:{id: GridMotoristas.idGrid}" cellspacing="0"></table>');

    _alterarVeiculoAcompanhamentoCarga?.CodigoMotorista?.val('');
    _alterarVeiculoAcompanhamentoCarga?.CPFMotorista?.val('');
}

function pesquisarVeiculosAlterarVeiculoAcompanhamentoCarga(e) {
    if (e.ModeloVeiculo.val().length == 0 && e.PlacaVeiculo.val().length == 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Preencha pelo menos um campo para pesquisa.");
        return;
    }
    _gridAlterarVeiculoAcompanhamentoCarga.CarregarGrid();
}

function pesquisarMotoristasAlterarVeiculo(e) {
    if (e.CodigoMotorista.val().length == 0 && e.CPFMotorista.val().length == 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Preencha pelo menos um campo para pesquisa.");
        return;
    }

    gridVeiculoAlterarMotoristaAcompanhamentoCarga(true);
}

function confirmarAlterarVeiculoAcompanhamentoCarga(e) {
    let codigosVeiculos = _gridAlterarVeiculoAcompanhamentoCarga.ObterMultiplosSelecionados().map(veiculo => { return veiculo.Codigo; });

    if (codigosVeiculos.length == 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Selecione ao menos um veículo para atualização.");
        return;
    }

    let codigosMotoristas = [];

    if (_alterarVeiculoAcompanhamentoCarga.ExibirAlterarMotoristaCarga.val()) {
        codigosMotoristas = _gridAlterarVeiculoAlterarMotoristaAcompanhamentoCarga.ObterMultiplosSelecionados().map(motorista => { return motorista.Codigo; });

        if (codigosMotoristas.length == 0) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Selecione ao menos um motorista para atualização.");
            return;
        }
    }

    executarReST("AcompanhamentoCarga/AlterarVeiculoAcompanhamentoCarga",
        {
            CodigoCarga: e.CodigoCarga.val(),
            CodigosVeiculos: JSON.stringify(codigosVeiculos),
            CodigosMotoristas: JSON.stringify(codigosMotoristas),
            AlterarMotoristaCarga: _alterarVeiculoAcompanhamentoCarga.ExibirAlterarMotoristaCarga.val()
        }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Data.Msg);
                if (retorno.Data.TemIntegracao)
                    visualizarIntegracoesAlterarVeiculoAcompanhamentoCarga();
                else
                    fecharModalAlterarVeiculoDaCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}
function DesabilitarFiltro(id) {
    _FiltroResumoAtendimentoSelecionado = EnumFiltroAtendimentosAcompanhamentoCarga.Nenhum;
    _FiltroResumoViagemSelecionado = EnumFiltroViagensAcompanhamentoCarga.Todas;
    _FiltroTendenciaPrazoEntrega = EnumFiltroTendenciaPrazoEntrega.Todos;
    $(".view-select-button.active").removeClass("active");

    BuscarCargasAcompanhamento(1, false).then(function () {
        AplicarConfigWidget();
        loadCargasNoMapa();
    });
}

function visualizarIntegracoesAlterarVeiculoAcompanhamentoCarga() {
    $("#etapaAlteracaoVeiculoAlterarVeiculoAcompanhamentoCarga").hide();
    $("#etapaAcompanhamentoIntegracoesAlterarVeiculoAcompanhamentoCarga").show();

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoAlterarVeiculoAcompanhamentoCarga, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico De Integracao", id: guid(), metodo: ExibirHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga, tamanho: "20", icone: "" });

    _gridIntegracaoAlterarVeiculoAcompanhamentoCarga = new GridView(_alterarVeiculoAcompanhamentoCarga.GridIntegracoes.idGrid, 'AcompanhamentoCarga/PesquisaIntegracoesAlterarVeiculoAcompanhamentoCarga', _alterarVeiculoAcompanhamentoCarga, menuOpcoes, null, 5);
    recarregarIntegracoesAlterarVeiculoAcompanhamentoCarga();
}

function recarregarIntegracoesAlterarVeiculoAcompanhamentoCarga() {
    _gridIntegracaoAlterarVeiculoAcompanhamentoCarga.CarregarGrid();
}

function ExibirHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga(integracao) {
    BuscarHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga");
}

function BuscarHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga(integracao) {
    let download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoAlterarVeiculoAcompanhamentoCargaClick, tamanho: "20", icone: "" };
    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    let _integracao = { Codigo: PropertyEntity({ type: types.int, val: ko.observable(integracao.Codigo) }) };
    _gridHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga = new GridView("tblHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga", "AcompanhamentoCarga/ConsultarHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga", _integracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga.CarregarGrid();
}

function ReenviarIntegracaoAlterarVeiculoAcompanhamentoCarga(data) {
    executarReST("AcompanhamentoCarga/ReenviarIntegracaoAlterarVeiculoAcompanhamentoCarga", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Reenvio Solicitado Com Sucesso");
            _gridIntegracaoAlterarVeiculoAcompanhamentoCarga.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function DownloadArquivosHistoricoIntegracaoAlterarVeiculoAcompanhamentoCargaClick(registroSelecionado) {
    executarDownload("AcompanhamentoCarga/DownloadArquivosHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga", { Codigo: registroSelecionado.Codigo, CodigoIntegracao: registroSelecionado.CodigoIntegracao });
}

function abrirModalAlterarMotoristaDaCarga(carga) {
    loadModalAlterarMotoristaDaCarga(carga);
}

function loadModalAlterarMotoristaDaCarga(cardCarga) {
    limparModalAlterarMotoristaDaCarga();

    _alterarMotoristaAcompanhamentoCarga = new AlterarMotoristaAcompanhamentoCarga(cardCarga);
    KoBindings(_alterarMotoristaAcompanhamentoCarga, "knockoutModalAlterarMotoristaAcompanhamentoCarga");

    _alterarMotoristaAcompanhamentoCarga.CodigoCarga.val(cardCarga.CodigoCarga.val());
    _alterarMotoristaAcompanhamentoCarga.Transportador.val(cardCarga.NomeTransportador.val());
    _alterarMotoristaAcompanhamentoCarga.CodigoTransportador.val(cardCarga.CodigoTransportador.val());
    _alterarMotoristaAcompanhamentoCarga.SubTitulo.val(_alterarMotoristaAcompanhamentoCarga.SubTitulo.val().replace("{1}", cardCarga.NomeTransportador.val()));

    Global.abrirModal("divModalAlterarMotoristaAcompanhamentoCarga");

    $("#etapaAlteracaoMotoristaAlterarMotoristaAcompanhamentoCarga").show();
    $("#etapaAcompanhamentoIntegracoesAlterarMotoristaAcompanhamentoCarga").hide();

    let multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: null,
        callbackSelecionado: null,
        callbackSelecionarTodos: null,
        somenteLeitura: false,
        permitirSelecionarSomenteUmRegistro: false,
        //manterSelecionadosMultiPesquisa: true
    };
    let ordenacaoPadrao = { column: 2, dir: orderDir.asc };

    _gridAlterarMotoristaAcompanhamentoCarga = new GridView(_alterarMotoristaAcompanhamentoCarga.GridMotoristas.idGrid, "AcompanhamentoCarga/PesquisarMotoristasAlterarMotoristaAcompanhamentoCarga", _alterarMotoristaAcompanhamentoCarga, null, ordenacaoPadrao, 5, null, null, null, multiplaEscolha);
}

function fecharModalAlterarMotoristaDaCarga() {
    limparModalAlterarMotoristaDaCarga();
    Global.fecharModal("divModalAlterarMotoristaAcompanhamentoCarga");
}
function limparModalAlterarMotoristaDaCarga() {
    let grid = $("#" + _alterarMotoristaAcompanhamentoCarga?.GridMotoristas?.idGrid + "_wrapper")
    if (grid)
        grid.html('<table width="100%" class="table table-bordered table-hover" data-bind="attr:{id: GridMotoristas.idGrid}" cellspacing="0"></table>');
    _gridAlterarMotoristaAcompanhamentoCarga = null;
    LimparCampos(_alterarMotoristaAcompanhamentoCarga);
}

function pesquisarMotoristasAlterarMotoristaAcompanhamentoCarga(e) {
    if (e.NomeMotorista.val().length == 0 && e.CPFMotorista.val().length == 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Preencha pelo menos um campo para pesquisa.");
        return;
    }
    _gridAlterarMotoristaAcompanhamentoCarga.CarregarGrid();
}

function confirmarAlterarMotoristaAcompanhamentoCarga(e) {
    let codigosMotoristas = _gridAlterarMotoristaAcompanhamentoCarga.ObterMultiplosSelecionados().map(motorista => { return motorista.Codigo; });
    if (codigosMotoristas.length == 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Selecione ao menos um motorista para atualização.");
        return;
    }

    executarReST("AcompanhamentoCarga/AlterarMotoristaAcompanhamentoCarga", { CodigoCarga: e.CodigoCarga.val(), CodigosMotoristas: JSON.stringify(codigosMotoristas) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Data.Msg);
                if (retorno.Data.temIntegracao)
                    visualizarIntegracoesAlterarMotoristaAcompanhamentoCarga();
                else
                    fecharModalAlterarMotoristaDaCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function visualizarIntegracoesAlterarMotoristaAcompanhamentoCarga() {
    $("#etapaAlteracaoMotoristaAlterarMotoristaAcompanhamentoCarga").hide();
    $("#etapaAcompanhamentoIntegracoesAlterarMotoristaAcompanhamentoCarga").show();

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoAlterarMotoristaAcompanhamentoCarga, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico De Integracao", id: guid(), metodo: ExibirHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga, tamanho: "20", icone: "" });

    _gridIntegracaoAlterarMotoristaAcompanhamentoCarga = new GridView(_alterarMotoristaAcompanhamentoCarga.GridIntegracoes.idGrid, 'AcompanhamentoCarga/PesquisaIntegracoesAlterarMotoristaAcompanhamentoCarga', _alterarMotoristaAcompanhamentoCarga, menuOpcoes, null, 5);
    recarregarIntegracoesAlterarMotoristaAcompanhamentoCarga();
}

function recarregarIntegracoesAlterarMotoristaAcompanhamentoCarga() {
    _gridIntegracaoAlterarMotoristaAcompanhamentoCarga.CarregarGrid();
}

function ExibirHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga(integracao) {
    BuscarHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga");
}
function BuscarHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga(integracao) {
    let download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoAlterarMotoristaAcompanhamentoCargaClick, tamanho: "20", icone: "" };
    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    let _integracao = { Codigo: PropertyEntity({ type: types.int, val: ko.observable(integracao.Codigo) }) };
    _gridHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga = new GridView("tblHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga", "AcompanhamentoCarga/ConsultarHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga", _integracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga.CarregarGrid();
}

function ReenviarIntegracaoAlterarMotoristaAcompanhamentoCarga(data) {
    executarReST("AcompanhamentoCarga/ReenviarIntegracaoAlterarMotoristaAcompanhamentoCarga", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Reenvio Solicitado Com Sucesso");
            _gridIntegracaoAlterarMotoristaAcompanhamentoCarga.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function DownloadArquivosHistoricoIntegracaoAlterarMotoristaAcompanhamentoCargaClick(registroSelecionado) {
    executarDownload("AcompanhamentoCarga/DownloadArquivosHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga", { Codigo: registroSelecionado.Codigo, CodigoIntegracao: registroSelecionado.CodigoIntegracao });
}