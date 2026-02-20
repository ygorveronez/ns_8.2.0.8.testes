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

var _DetalhesCargaEspelhada;
var _gridAlertasCargaEspelhada;
var _containerControleEntrega;

function loadDetalhesCargaEspelhada() {
    _DetalhesCargaEspelhada = new DetalhesCargaEspelhada();
    KoBindings(_DetalhesCargaEspelhada, "knockoutDetalhesCargaEspelhada");
}

var DetalhesCargaEspelhada = function () {
    this.CargaEmbarcadorEspelhada = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.PlacaVeiculoEspelhada = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.ETAUltimaEntregaEspelhada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ProximaEntregaDataReprogramada, val: ko.observable(""), visible: ko.observable(true) });
    this.UltimaEntregaEspelhadaAtrasada = PropertyEntity({ val: ko.observable(false) });
    this.DataCarregamentoEspelhada = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true) });
    this.Entregas = PropertyEntity({ type: types.local, val: ko.observableArray([]) });
};


function loadGridAlertas(carga, callback) {
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    _gridAlertasCargaEspelhada = new GridView("grid-alertas-carga-espelhada", "Monitoramento/ObterAlertas?carga=" + carga + "&DescricaoAlerta=Sem alerta", null, null, null, totalRegistrosPorPagina, null, false, false, null, limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridAlertasCargaEspelhada.CarregarGrid(callback);
}

function ExibirModalDetalhesCargaEspelhada() {
    Global.abrirModal('divModalDetalhesCargaEspelhada');
}


//buscar os dados dos alertas (seja alerta de monitoramento ou Alerta de carga) e tratar individualmente
function detalhesCargaEspelhadaClick(dadosCard) {
    if (dadosCard.PossuiMonitoramentoAtivoProVeiculoEmOutraCarga.val()) {
        var codigoCarga = dadosCard.CodigoCargaEspelhadaComMonitoramentoAtivo.val();
        if (codigoCarga > 0) {
            executarReST("AcompanhamentoCarga/DetalhesCargaEspelhada", { CodigoCarga: codigoCarga }, function (arg) {
                if (arg.Success) {
                    var data = arg.Data;
                    if (data !== false) {
                        _DetalhesCargaEspelhada.CargaEmbarcadorEspelhada.val(data.CargaEmbarcador);
                        _DetalhesCargaEspelhada.PlacaVeiculoEspelhada.val(data.PlacaVeiculo);
                        _DetalhesCargaEspelhada.ETAUltimaEntregaEspelhada.val(data.ETAUltimaEntrega);
                        _DetalhesCargaEspelhada.UltimaEntregaEspelhadaAtrasada.val(data.UltimaEntregaEspelhadaAtrasada);
                        _DetalhesCargaEspelhada.DataCarregamentoEspelhada.val(data.DataCarregamentoEspelhada);
                        loadGridAlertas(codigoCarga, ExibirModalDetalhesCargaEspelhada);

                        visualizarDetalhesEntrega(codigoCarga);
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

function visualizarDetalhesEntrega(filaSelecionada) {
    var carga = filaSelecionada;

    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não existe carga para este veículo.");
        return;
    }

    executarReST("/ControleEntrega/ObterControleEntregaPorcarga", { Carga: carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _containerControleEntrega = new ContainerControleEntrega();
                KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");
                _containerControleEntrega.Entregas.val([arg.Data.Entregas]);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}