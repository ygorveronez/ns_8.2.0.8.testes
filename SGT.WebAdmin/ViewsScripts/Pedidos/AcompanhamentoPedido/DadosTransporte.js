/// <reference path="../../logistica/tracking/tracking.lib.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />

var _dadosTransporte;
var _gridDadosTransporteHistoricoPedido;
var _gridDadosTransporteHistoricoAduana;
var _mapDadosTransporte;
var _mapaMonitoramento;
/*
 * Declaração das Funções Associadas a Eventos
 */

var DadosTransporte = function (id) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LocalTransporte = PropertyEntity({ type: types.local });


    this.knockoutId = PropertyEntity({ val: "knockout-dados-Transporte-" + id });
    this.HistoricoPedido = PropertyEntity({ type: types.local, idGrid: guid() });
    this.HistoricosAduana = PropertyEntity({ type: types.local, idGrid: guid() });
    this.PossuiPassagemAduanas = PropertyEntity({ val: ko.observable(false), def: false });
    this.DadosTransporte = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DadosTransporte });
    this.Historico = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Historico });
}

function RegistraComponenteAcompanhamentoTransporte() {
    if (ko.components.isRegistered('acompanhamento-Transporte'))
        return;

    ko.components.register('acompanhamento-Transporte', {
        viewModel: EtapaAcompanhamentoTransporte,
        template: {
            element: 'acompanhamento-Transporte-templete'
        }
    });
}

function loadDadosTransporte(e) {
    var knockout = "knockout-dados-Transporte-" + e.EtapaTransporte.idTab;

    _dadosTransporte = new DadosTransporte(e.EtapaTransporte.idTab);
    KoBindings(_dadosTransporte, knockout);
}



/*
 * Declaração das Funções
 */

function exibirDadosEtapaTransporte(e) {
    fecharDados();
    loadDadosTransporte(e);
    loadGridDadosTransporteHistoricoPedido(e.CodigoPedido.val());
    loadGridDadosTransporteHistoricoAduana(e.CodigoPedido.val());

    loadMapa();
    obterDadosTransporte(e.CodigoPedido.val());

    _dadosTransporte.PossuiPassagemAduanas.val(e.PossuiPassagemAduanas.val());

    $("#" + e.EtapaTransporte.idTab).addClass("active show");
}

function loadMapa() {

    //if (!_mapaMonitoramento) {
    var opcoesmapa = new OpcoesMapa(false, false);
    _mapaMonitoramento = new MapaGoogle("map-" + _dadosTransporte.knockoutId.val, false, opcoesmapa);
    //}
}

function obterDadosTransporte(codigoPedido) {
    executarReST("AcompanhamentoPedido/ObterDadosTransporte", { CodigoPedido: codigoPedido }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                var dados = {
                    Carga: arg.Data.DadosTransporte.Carga,
                    Veiculo: arg.Data.DadosTransporte.Veiculo,
                    Monitoramento: arg.Data.DadosTransporte.Monitoramento
                };

                //SetarCoordenadasDadosTransporte(positionTransporte);
                carregarDadosMapa(dados);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function carregarDadosMapa(dados) {
    _mapaMonitoramento.direction.limparMapa();
    _mapaMonitoramento.clear();

    executarReST("Monitoramento/ObterDadosMapa", {
        Codigo: dados.Monitoramento,
        Carga: dados.Carga,
        Veiculo: dados.Veiculo,
        IDEquipamento: 0
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {
                    TrackingDesenharInformacoesMapa(_mapaMonitoramento, arg.Data);
                    TrackingCriarMarkerVeiculo(_mapaMonitoramento, arg.Data.Veiculo, false, 0)
                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function loadGridDadosTransporteHistoricoPedido(pedido) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    _gridDadosTransporteHistoricoPedido = null;

    //_gridDadosTransporteHistoricoPedido = new GridView("grid-dados-transporte-historico-pedido", "Pedido/ObterHistoricoPedido?pedido=" + pedido, null, null, null, totalRegistrosPorPagina, null, false, draggableRows, null, limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridDadosTransporteHistoricoPedido = new GridView(_dadosTransporte.HistoricoPedido.idGrid, "Pedido/ObterHistoricoPedido?pedido=" + pedido, null, null, null, totalRegistrosPorPagina, null, false, draggableRows, null, limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridDadosTransporteHistoricoPedido.CarregarGrid();
}

function loadGridDadosTransporteHistoricoAduana(pedido) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    _gridDadosTransporteHistoricoAduana = null;

    _gridDadosTransporteHistoricoAduana = new GridView(_dadosTransporte.HistoricosAduana.idGrid, "Pedido/ObterHistoricoAduanaPedido?pedido=" + pedido, null, null, null, totalRegistrosPorPagina, null, false, draggableRows, null, limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridDadosTransporteHistoricoAduana.CarregarGrid();
}

function SetarCoordenadasDadosTransporte(positionEntrega) {
    CarregarMapaDadosTransporte();

    if ((positionEntrega.lat == 0) && (positionEntrega.lng == 0))
        return;

    var marker = new ShapeMarker();
    marker.setPosition(positionEntrega.lat, positionEntrega.lng);
    marker.title = '';

    _mapDadosTransporte.draw.addShape(marker);
    _mapDadosTransporte.direction.setZoom(17);
    _mapDadosTransporte.direction.centralizar(positionEntrega.lat, positionEntrega.lng);
}

function CarregarMapaDadosTransporte() {

    if (_mapDadosTransporte == null) {
        opcoesMapa = new OpcoesMapa(false, false);

        _mapDadosTransporte = new MapaGoogle(_dadosTransporte.LocalTransporte.id, false, opcoesMapa);
    }

    _mapDadosTransporte.clear();
}
