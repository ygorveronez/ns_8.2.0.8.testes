/*Monitoramento.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatusViagemTipoRegra.js" />
/// <reference path="../../Enumeradores/EnumAlertaMonitorStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoFiltroCliente.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../Tracking/Tracking.lib.js" />

var _gridTorreMonitoramento;
var _gridAlertas;
var _pesquisaTorreMonitoramento;
var _tabelaAlertaResumo;
var _alertaDetalhe;
var _mapaMonitoramento;
var _CRUDTratativaAlerta;
var _gridResumoCarga;
var _gridMapaParadas;

var tiposFiltroCliente = [
    { text: "Nenhum", value: 0 },
    { text: "Estão em alvo", value: this.Iniciado },
    { text: "Possui entrega", value: this.Finalizado },
    { text: "Cancelado", value: this.Cancelado }
];

/*
 * Declaração das Classes
 */

var PesquisaTorreMonitoramento = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaTorreMonitoramento)) {
                _pesquisaTorreMonitoramento.ExibirFiltros.visibleFade(false);
                _pesquisaTorreMonitoramento.DescricaoAlerta.val("");
                recarregarDados();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: ", col: 12 });
    this.Veiculo = PropertyEntity({ text: "Veículos:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: "Pedido: ", col: 12 });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Nota Fiscal: ", col: 12 });
    this.MonitoramentoStatus = PropertyEntity({ text: "Status Monitoramento: ", val: ko.observable(EnumMonitoramentoStatus.Iniciado), options: EnumMonitoramentoStatus.obterOpcoesPesquisa(), def: EnumMonitoramentoStatus.Iniciado });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.FuncionarioVendedor = PropertyEntity({ text: "Vendedor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusViagem = PropertyEntity({ text: "Status da Viagem: ", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.GrupoTipoOperacao = PropertyEntity({ text: "Tipo de operação: ", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.DescricaoAlerta = PropertyEntity();
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    if (_CONFIGURACAO_TMS.TelaMonitoramentoPadraoFiltroDataInicialFinal) {
        this.DataInicial.val(Global.Data(EnumTipoOperacaoDate.Subtract, 2, EnumTipoOperacaoObjetoDate.Days));
        this.DataFinal.val(Global.Data(EnumTipoOperacaoDate.Add, 3, EnumTipoOperacaoObjetoDate.Days));
    }
    this.FiltroCliente = PropertyEntity({ text: "Filtrar clientes: ", val: ko.observable(EnumMonitoramentoFiltroCliente.Nenhum), options: EnumMonitoramentoFiltroCliente.obterOpcoes(), def: EnumMonitoramentoFiltroCliente.Nenhum });
    this.Cliente = PropertyEntity({ text: "Cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.CategoriaPessoa = PropertyEntity({ text: "Categoria:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.SomenteRastreados = PropertyEntity({ val: ko.observable(true), text: "Somente veículos com rastreador? ", getType: typesKnockout.bool });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.ClienteOrigem = PropertyEntity({ text: "Origem (Expedidor):", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ClienteDestino = PropertyEntity({ text: "Destino (Recebedor):", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisaRelatorio(), def: EnumSituacaoCargaJanelaCarregamento.Todas, text: "Situação Janela Carregamento: " });
    this.NumeroEXP = PropertyEntity({ text: "Número EXP: ", maxlength: 150 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntregaPedidoInicio = PropertyEntity({ text: "Data entrega pedido Inicial: ", getType: typesKnockout.dateTime, val: ko.observable() });
    this.DataEntregaPedidoFinal = PropertyEntity({ text: "Data entrega pedido Final: ", getType: typesKnockout.dateTime, val: ko.observable() });
    this.PrevisaoEntregaInicio = PropertyEntity({ text: "Previsão entrega planejada inicio: ", getType: typesKnockout.dateTime, val: ko.observable() });
    this.PrevisaoEntregaFinal = PropertyEntity({ text: "Previsão entrega planejada final: ", getType: typesKnockout.dateTime, val: ko.observable() });
    this.VeiculosComContratoDeFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Apenas veículos que possuem contrato de frete", visible: ko.observable(true) });
    this.Destino = PropertyEntity({ text: "Destino:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
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
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CRUDTratativaAlerta = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarTratativaClick, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarTratativaClick, text: "Cancelar", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.CodigoAlerta = PropertyEntity();
};

var PesquisaHistoricoPosicao = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaHistoricoPosicao))
                carregarDadosMapaHistoricoPosicao();
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.PosicaoTemperaturaValida = PropertyEntity({ text: "Apenas posições com temperatura enviada", val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });
};

var AlertaDetalhe = function () {
    this.Codigo = PropertyEntity({ text: "Código: " });
    this.DataCadastro = PropertyEntity({ text: "Data cadastro: " });
    this.Data = PropertyEntity({ text: "Data: " });
    this.Tipo = PropertyEntity({ text: "Tipo do alerta: " });
    this.AlertaDescricao = PropertyEntity({ text: "Valor: " });
    this.Coordenadas = PropertyEntity({ text: "Coordenadas: " });
    this.Carga = PropertyEntity({ text: "Carga: " });
    this.Status = PropertyEntity({ text: "Status: " });
    this.StatusDescricao = PropertyEntity({ text: "Status: " });
    this.Observacao = PropertyEntity({ text: "Observacao: " });
};

var TabelaAlertaResumo = function () {
    this.ListaCabecalho = ko.observableArray([]);
    this.ListaDadosTotais = ko.observableArray([]);
    this.ListaDadosCargas = ko.observableArray([]);
    this.ListaDadosPendentes = ko.observableArray([]);
};

function PesquisarPorEvento(col) {
    _pesquisaTorreMonitoramento.DescricaoAlerta.val(col.text);
    _gridTorreMonitoramento.CarregarGrid();
}

/*
 * Declaração das Funções de Inicialização
 */
function loadDroppable() {
    $("#container-grid-torre-monitoramento").droppable({
        drop: itemSoltado,
        hoverClass: "ui-state-active backgroundDropHover",
    });
}

function loadTorreMonitoramento() {
    loadPesquisaMonitoramento();

    _tabelaAlertaResumo = new TabelaAlertaResumo();
    KoBindings(_tabelaAlertaResumo, "knoutContainerTabelaAlerta");

    buscarDetalhesOperador(function () {
        buscaStatusViagem(function () {
            loadGridMonitoramento();
            loadTabelaAlertaResumo();

            loadDetalhesCarga();
            loadDroppable();
            loadCRUDTratativaAlerta();
            loadDetalhesMonitoramento();
            loadHistoricoMonitoramento();
            loadAlertaDetalhe();

            new BuscarTransportadores(_pesquisaTorreMonitoramento.Transportador, null, null, true);
            new BuscarClientes(_pesquisaTorreMonitoramento.Cliente);
            new BuscarCategoriaPessoa(_pesquisaTorreMonitoramento.CategoriaPessoa);
            new BuscarGruposPessoas(_pesquisaTorreMonitoramento.GrupoPessoa);
            new BuscarFilial(_pesquisaTorreMonitoramento.Filial);
            new BuscarClientes(_pesquisaTorreMonitoramento.ClienteDestino);
            new BuscarClientes(_pesquisaTorreMonitoramento.ClienteOrigem);
            new BuscarVeiculos(_pesquisaTorreMonitoramento.Veiculo);
            new BuscarFuncionario(_pesquisaTorreMonitoramento.FuncionarioVendedor);
            new BuscarTiposOperacao(_pesquisaTorreMonitoramento.TipoOperacao);
            new BuscarClientes(_pesquisaTorreMonitoramento.Expedidor);
            new BuscarLocalidades(_pesquisaTorreMonitoramento.Destino);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaTorreMonitoramento.Filial.visible(false);
                _pesquisaTorreMonitoramento.Transportador.visible(false);
                _pesquisaTorreMonitoramento.GrupoPessoa.visible(true);
                _pesquisaTorreMonitoramento.VeiculosComContratoDeFrete.visible(false);
            }

            var cssClass = "col col-xs-12 col-sm-2 col-md-2 col-lg-2";
            if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento) {
                buscaGrupoTipoOperacao();
                _pesquisaTorreMonitoramento.GrupoTipoOperacao.visible(true);
                if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
                    _pesquisaTorreMonitoramento.StatusViagem.visible(false);
                } else {
                    _pesquisaTorreMonitoramento.StatusViagem.visible(true);
                    cssClass = "col col-xs-12 col-sm-1 col-md-1 col-lg-1";
                }
            } else {
                _pesquisaTorreMonitoramento.GrupoTipoOperacao.visible(false);
            }
            _pesquisaTorreMonitoramento.DataInicial.cssClass(cssClass);
            _pesquisaTorreMonitoramento.DataFinal.cssClass(cssClass);

            loadMonitoramentoControleEntrega(function () {
                registraComponente();
                loadEtapasControleEntrega();

                isMobile = $(window).width() <= 980;
                _containerControleEntrega = new ContainerControleEntrega();
                KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");
            });

            $("#" + _pesquisaTorreMonitoramento.FiltroCliente.id).change(verificarPesquisaFiltroCliente);
        });
    });
}

function loadTabelaAlertaResumo() {

    executarReST(
        "Monitoramento/ObterAlertasTorre",
        RetornarObjetoPesquisa(_pesquisaTorreMonitoramento),
        function (arg) {
            if (arg.Success) {
                if (arg.Data) {

                    _tabelaAlertaResumo.ListaCabecalho.removeAll();
                    _tabelaAlertaResumo.ListaDadosCargas.removeAll();
                    _tabelaAlertaResumo.ListaDadosPendentes.removeAll();
                    _tabelaAlertaResumo.ListaDadosTotais.removeAll();

                    _tabelaAlertaResumo.ListaCabecalho(arg.Data.ListaCabecalho);
                    _tabelaAlertaResumo.ListaDadosCargas(arg.Data.ListaDadosCargas);
                    _tabelaAlertaResumo.ListaDadosPendentes(arg.Data.ListaDadosPendentes);
                    _tabelaAlertaResumo.ListaDadosTotais(arg.Data.ListaDadosTotais);

                    //PreencherObjetoKnout(_tabelaAlertaResumo, arg.Data);
                    //gerarTabelaAlertas(arg.Data);

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }
    );
}

function verificarPesquisaFiltroCliente() {
    var categoria = false, cliente = false;
    if (_pesquisaTorreMonitoramento.FiltroCliente.val() != EnumMonitoramentoFiltroCliente.Nenhum) {
        categoria = true;
        cliente = true;
    }
    _pesquisaTorreMonitoramento.CategoriaPessoa.enable(categoria);
    _pesquisaTorreMonitoramento.Cliente.enable(cliente);
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
    _pesquisaTorreMonitoramento = new PesquisaTorreMonitoramento();
    KoBindings(_pesquisaTorreMonitoramento, "knockoutPesquisaMonitoramento", false, _pesquisaTorreMonitoramento.Pesquisar.id);
}

function loadGridMonitoramento() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 100;
    var totalRegistrosPorPagina = 100;
    var opcaoDetalheMonitoramento = { descricao: "Detalhes do monitoramento", id: guid(), evento: "onclick", metodo: visualizarDetalhesMonitoramentoClick, tamanho: "10", icone: "" };
    var opcaoAlertas = { descricao: "Detalhes dos alertas", id: guid(), evento: "onclick", metodo: visualizarAlertasClick, tamanho: "10", icone: "" };
    var opcaoResumoCarga = { descricao: "Resumo das entregas", id: guid(), evento: "onclick", metodo: visualizarResumoDaCargaClick, tamanho: "10", icone: "" };
    var opcaoDetalhesCarga = { descricao: "Detalhes da carga", id: guid(), evento: "onclick", metodo: visualizarDetalhesCargaClick, tamanho: "10", icone: "" };
    var opcaoDetalhesEntrega = { descricao: "Detalhes da entrega", id: guid(), evento: "onclick", metodo: visualizarDetalhesEntregaClick, tamanho: "10", icone: "" };
    var opcaoVisualizarMapa = { descricao: "Visualizar no mapa", id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
    var opcaoHistoricos = { descricao: "Históricos", id: guid(), evento: "onclick", metodo: visualizarHistoricosClick, tamanho: "8", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoDetalheMonitoramento, opcaoAlertas, opcaoDetalhesCarga, opcaoResumoCarga, opcaoDetalhesEntrega, opcaoVisualizarMapa, opcaoHistoricos], tamanho: 5, };
    var configuracoesExportacao = { url: "Monitoramento/ExportarPesquisaTorreMonitoramento", titulo: "Torre Monitoramento" };

    _gridTorreMonitoramento = new GridView("grid-torre-monitoramento", "Monitoramento/PesquisaTorreMonitoramento", _pesquisaTorreMonitoramento, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao, undefined, undefined, gridMonitoramentoCallbackRow, gridMonitoramentoCallbackColumnDefault);
    _gridTorreMonitoramento.SetPermitirEdicaoColunas(true);
    _gridTorreMonitoramento.SetSalvarPreferenciasGrid(true);
    _gridTorreMonitoramento.SetHabilitarScrollHorizontal(true, 200);
    _gridTorreMonitoramento.CarregarGrid();


}

function loadGridAlertas(carga) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;
    var opcaoTratativaAlerta = { descricao: "Tratativa alerta", id: guid(), evento: "onclick", metodo: tratativaAlertaClick, tamanho: "10", icone: "", visibilidade: visualizarAlertaTratativaVisibilidade };
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: visualizarAlertaDetalhesClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoTratativaAlerta, opcaoDetalhes], tamanho: 10, };

    _gridAlertas = new GridView("grid-alertas", "Monitoramento/ObterAlertasTorreGrid?carga=" + carga + "&DescricaoAlerta=" + _pesquisaTorreMonitoramento.DescricaoAlerta.val(), null, menuOpcoes, null, totalRegistrosPorPagina, null, false, draggableRows, null,
        limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridAlertas.CarregarGrid();
}

function loadGridResumoCarga(carga) {
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    _gridResumoCarga = new GridView("grid-resumo-carga", "Monitoramento/ObterResumoCarga?carga=" + carga, null, null, { column: 0, dir: orderDir.asc }, totalRegistrosPorPagina, null, false, false, null,
        limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridResumoCarga.CarregarGrid();
}

function loadAlertaDetalhe() {
    _alertaDetalhe = new AlertaDetalhe();
    KoBindings(_alertaDetalhe, "knockoutAlertaDetalhe");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function ExibirModalMapa() {
    $(".legenda-rotas-container").hide();
    Global.abrirModal('divModalMapa');
    $("#divModalMapa").one('hidden.bs.modal', function () {
        _mapaMonitoramento.direction.limparMapa();
    });
}

function ExibirModalAlertas() {
    Global.abrirModal('divModalAlerta');
}

function ExibirModalResumoCarga() {
    Global.abrirModal('divModalResumoCarga');
}

function ExibirModalTratativaAlerta() {
    Global.abrirModal('divModalTratativaAlerta');
}

function visualizarAlertasClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ExibirModalAlertas()
    loadGridAlertas(filaSelecionada.Carga);
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
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
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
    executarReST("Monitoramento/ObterDetalhesAlertaTorre", {
        Codigo: row.Codigo
    }, function (retorno) {
        if (retorno.Success) {
            Global.abrirModal('divModalAlertaDetalhe');
            PreencherObjetoKnout(_alertaDetalhe, retorno);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function visualizarAlertaTratativaVisibilidade(row) {
    return (row.Status == EnumAlertaMonitorStatus.EmAberto);
}

function visualizarDetalhesMonitoramentoClick(filaSelecionada) {
    exibirDetalhesMonitoramentoPorCodigo(filaSelecionada.Codigo);
}

function limparCamposTratativa() {
    LimparCampos(_CRUDTratativaAlerta);
}

function cancelarTratativaClick() {
    Global.fecharModal("divModalTratativaAlerta");

    limparCamposTratativa();
}

function confirmarTratativaClick(e, sender) {
    Salvar(_CRUDTratativaAlerta, "Monitoramento/AdicionarTratativaAlertaCarga", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                Global.fecharModal("divModalTratativaAlerta");
                limparCamposTratativa();
                _gridAlertas.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function visualizarDetalhesEntregaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    var carga = filaSelecionada.Carga;

    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não existe carga para este veículo.");
        return;
    }

    executarReST("/ControleEntrega/ObterControleEntregaPorcarga", { Carga: carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.abrirModal('knoutContainerControleEntrega');
                $("#knoutContainerControleEntrega").one('hidden.bs.modal', function () {
                });

                _containerControleEntrega.Entregas.val([arg.Data.Entregas]);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}

function loadDetalhesCarga() {
    buscarDetalhesOperador(function () {

        carregarConteudosHTML(function () {

        });

    });
}

function visualizarDetalhesCargaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ObterDetalhesCargaFluxo(filaSelecionada.Carga);
}

function ObterDetalhesCargaFluxo(carga) {
    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não existe carga para este veículo.");
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
                Global.abrirModal('divModalDetalhesCarga');
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */
function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function itemSoltado(event, ui) {
    var idContainerDestino = event.target.id;
    var idContainerOrigem = "container-" + $(ui.draggable[0]).parent().parent()[0].id;

}

function recarregarDados() {
    _gridTorreMonitoramento.CarregarGrid();
    loadTabelaAlertaResumo();
}

function tratativaAlertaClick(filaSelecionada) {
    _CRUDTratativaAlerta.CodigoAlerta.val(filaSelecionada.Codigo);
    ExibirModalTratativaAlerta();
}

function atualizaTituloModalCarga(row) {
    $(".title-carga-codigo-embarcador").html(row.CargaEmbarcador);
    $(".title-carga-placa").html(row.Tracao + " " + row.Reboques);
}

function buscaStatusViagem(callback) {
    executarReST("MonitoramentoStatusViagem/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var selected = [];
                for (var i = 0; i < arg.Data.StatusViagem.length; i++) {
                    if (arg.Data.StatusViagem[i].selected == 'selected') {
                        selected.push(arg.Data.StatusViagem[i].value);
                    }
                }
                _pesquisaTorreMonitoramento.StatusViagem.options(arg.Data.StatusViagem);
                _pesquisaTorreMonitoramento.StatusViagem.val(selected);

                $("#" + _pesquisaTorreMonitoramento.StatusViagem.id).selectpicker('refresh');

                callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscaGrupoTipoOperacao() {
    executarReST("GrupoTipoOperacao/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _pesquisaTorreMonitoramento.GrupoTipoOperacao.options(arg.Data.GrupoTipoOperacao);

                $("#" + _pesquisaTorreMonitoramento.GrupoTipoOperacao.id).selectpicker('refresh');
                
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
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
