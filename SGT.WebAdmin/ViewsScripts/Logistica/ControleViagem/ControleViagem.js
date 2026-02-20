/*ControleViagem.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../Tracking/Tracking.lib.js" />

var _gridControleViagem;
var _pesquisaControleViagem;
var _playerControleViagem;
var _timerInterval = null;
var _totalSeconds = 60;
var _countSeconds = _totalSeconds;

var rastreadorCorOnline = "#33cc33";
var rastreadorCorOffline = "#e74c3c";

var _mapaControleViagem;

/*
 * Declaração das Classes
 */

var PesquisaControleViagem = function () {
    this.Pesquisar = PropertyEntity({
        type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true),
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaControleViagem)) {
                _pesquisaControleViagem.ExibirFiltros.visibleFade(false);
                _gridControleViagem.CarregarGrid();
            } else {
                exibirMensagemCamposObrigatorio();
            }
        }
    });
    this.DataInicialCarga = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataFinalCarga = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataInicialCarga.dateRangeLimit = this.DataFinalCarga;
    this.DataFinalCarga.dateRangeInit = this.DataInicialCarga;
    this.Filial = PropertyEntity({ text: "Filial:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador:"), type: types.multiplesEntities, codEntity: ko.observable(0), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículos:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: "Destino:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.StatusViagem = PropertyEntity({ text: "Status da Viagem: ", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), params: { Tipo: "", Ativo: ([]).Todas, OpcaoSemGrupo: false }});

    this.ExibirFiltros = PropertyEntity({
        type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true),
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }
    });
};

var PlayerControleViagem = function () {
    this.Play = PropertyEntity({
        type: types.event, text: "Play", idGrid: guid(),
        eventClick: function (e) {
            if (_timerInterval == null) {
                $('#' + _playerControleViagem.Play.id).addClass("disable");
                $('#' + _playerControleViagem.Pause.id).removeClass("disable");
                _timerInterval = setInterval(function () {
                    if (_countSeconds == 0) {
                        _countSeconds = _totalSeconds + 1;
                        recarregarDados();
                    }
                    _countSeconds--;
                    $('#knockoutPlayerControleViagem span.count-seconds').html(_countSeconds);
                }, 1000);
            }
        }
    });
    this.Pause = PropertyEntity({
        type: types.event, text: "Pause", idGrid: guid(),
        eventClick: function (e) {
            if (_timerInterval != null) {
                $('#' + _playerControleViagem.Play.id).removeClass("disable");
                $('#' + _playerControleViagem.Pause.id).addClass("disable");
                clearInterval(_timerInterval);
                _timerInterval = null;
            }
        }
    });
    this.Refresh = PropertyEntity({
        type: types.event, text: "Atualizar", idGrid: guid(),
        eventClick: function (e) {
            recarregarDados();
        }
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadControleViagem() {
    loadPesquisaControleViagem();
    buscaStatusViagem(loadControleViagemGoGo);
}

function loadControleViagemGoGo() {
    
    new BuscarFilial(_pesquisaControleViagem.Filial);
    new BuscarTransportadores(_pesquisaControleViagem.Transportador, null, null, true);
    new BuscarVeiculos(_pesquisaControleViagem.Veiculo);
    new BuscarClientes(_pesquisaControleViagem.Cliente);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaControleViagem.Filial.visible(false);
        _pesquisaControleViagem.Transportador.visible(false);
    }
    loadPlayeraControleViagem();
    loadGridControleViagem();
}

function loadPesquisaControleViagem() {
    _pesquisaControleViagem = new PesquisaControleViagem();
    KoBindings(_pesquisaControleViagem, "knockoutPesquisaControleViagem", false, _pesquisaControleViagem.Pesquisar.id);
}

function loadGridControleViagem() {
    var opcaoVisualizarMapa = { descricao: "Visualizar no mapa", id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoVisualizarMapa], tamanho: 5, };
    _gridControleViagem = new GridView("grid-controle-viagem", "ControleViagem/Pesquisa", _pesquisaControleViagem, menuOpcoes, null, 25, null, true, false, undefined, 25, undefined, undefined, undefined, undefined, callbackRowGridControleViagem);
    _gridControleViagem.SetPermitirEdicaoColunas(true);
    _gridControleViagem.SetPermitirReordenarColunas(false);
    _gridControleViagem.SetSalvarPreferenciasGrid(true);
    _gridControleViagem.SetHabilitarScrollHorizontal(true, 200);
    _gridControleViagem.CarregarGrid();
}

function loadPlayeraControleViagem() {
    $('#knockoutPlayerControleViagem span.count-seconds').html(_countSeconds);
    _playerControleViagem = new PlayerControleViagem();
    KoBindings(_playerControleViagem, "knockoutPlayerControleViagem", false, _playerControleViagem.Play.id);
    $('#' + _playerControleViagem.Pause.id).addClass("disable");
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
                _pesquisaControleViagem.StatusViagem.options(arg.Data.StatusViagem);
                
                $("#" + _pesquisaControleViagem.StatusViagem.id).selectpicker('refresh');

                callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function callbackRowGridControleViagem(nRow, aData) {
    gerarColunaRastreadorGridControleViagem(nRow, aData)
}

function gerarColunaRastreadorGridControleViagem(nRow, aData) {
    var indice = _gridControleViagem.GetColumnIndex('Rastreador');
    if (indice == undefined) return;
    var colunaRastreador = $(nRow).find('td').eq(indice);
    if (colunaRastreador) {
        var icone = TrackingIconRastreador(aData.Rastreador);
        var html = '<div>' + icone + '</div>';
        $(colunaRastreador).html(html);
        $(colunaRastreador).addClass('tracking-indicador');
        $(colunaRastreador).attr('title', aData.PosicaoDataVeiculo);
    }
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function recarregarDados() {
    _gridControleViagem.proximaPagina(true);
}

function visualizarMapaClick(row) {
    $(".title-carga-codigo-embarcador").html(row.CargaCodigoEmbarcador);
    $(".title-carga-placa").html(row.VeiculoTracaoPlaca);

    $(".legenda-rotas-container").hide();
    Global.abrirModal('divModalMapa');
    $("#divModalMapa").one('hidden.bs.modal', function () {
        _mapaControleViagem.direction.limparMapa();
    });

    if (!_mapaControleViagem) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaControleViagem = new MapaGoogle("map", false, opcoesmapa);
    }

    _mapaControleViagem.clear();
    executarReST("ControleViagem/ObterDadosMapa", {
        Codigo: row.MonitoramentoCodigo
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {
                    TrackingDesenharInformacoesMapa(_mapaControleViagem, arg.Data);
                    TrackingCriarMarkerVeiculo(_mapaControleViagem, arg.Data.Veiculo, false, 0)
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}
