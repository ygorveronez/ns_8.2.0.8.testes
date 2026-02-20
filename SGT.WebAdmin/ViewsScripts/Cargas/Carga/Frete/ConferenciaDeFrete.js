/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

var _conferenciaDeFrete;
var _gridConferenciaDeFrete;

function ConferenciaDeFrete() {

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    //this.ValorTotalLiquido = PropertyEntity({ text: "Valor Total Frete Líquido", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });

    this.GridConferenciaDeFrete = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.AplicarAjusteConferenciaDeFrete = PropertyEntity({ eventClick: AplicarAjusteConferenciaDeFreteClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.AplicarAjuste });
    this.ConfirmarConferenciaDeFrete = PropertyEntity({ eventClick: ConfirmarConferenciaDeFreteClick, enable: ko.observable(true), visible: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarConferenciaDeFrete });
    this.ListaConferenciaFrete = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var ConferenciaDeFreteMap = function () {
    this.CodigosPedidoXML = PropertyEntity({ val: 0, def: 0 });
    this.Remetente = PropertyEntity({ val: 0, def: 0 });
    this.Destinatario = PropertyEntity({ val: 0, def: 0 });
    this.ValorLiquido = PropertyEntity({ val: 0, def: 0 });
};

function CarregarGridConferenciaDeFrete(e) {
    _conferenciaDeFrete = new ConferenciaDeFrete();
    KoBindings(_conferenciaDeFrete, "knockoutConferenciaDeFrete");

    _conferenciaDeFrete.Codigo.val(e.Codigo.val());

    var menuOpcoes = null;
    var editarColuna = { permite: true, callback: CallBackEditarValor, atualizarRow: false };

    _gridConferenciaDeFrete = new GridView(_conferenciaDeFrete.GridConferenciaDeFrete.idGrid, "CargaFrete/PesquisaConferenciaDeFrete", _conferenciaDeFrete, menuOpcoes, null, 10, null, null, null, null, null, editarColuna, null);
    _gridConferenciaDeFrete.CarregarGrid();
}

function CallBackEditarValor(dataRow, row, head, callbackTabPress) {
    if (_conferenciaDeFrete.AplicarAjusteConferenciaDeFrete.visible() == false) {
        _conferenciaDeFrete.AplicarAjusteConferenciaDeFrete.visible(true);
        _conferenciaDeFrete.ConfirmarConferenciaDeFrete.enable(false);
    }

    var data = {
        Codigo: _conferenciaDeFrete.Codigo.val()
        , CodigosPedidoXML: dataRow.CodigosPedidoXML
        , NumeroPedido: dataRow.NumeroPedido
        , Remetente: dataRow.Remetente
        , Destinatario: dataRow.Destinatario
        , ValorLiquido: dataRow.ValorLiquido
        , Percentual: dataRow.Percentual
        , ValorComponentes: dataRow.ValorComponentes
        , ValorImpostos: dataRow.ValorImpostos
        , ValorTotal: dataRow.ValorTotal
        //, ValorTotalLiquido: _gridConferenciaDeFrete.ValorTotalLiquido.val()
    };

    executarReST("CargaFrete/AlterarFreteConferenciaDeFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data.dynAtualizarLinhaConferenciaDeFrete);
                _gridConferenciaDeFrete.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                _gridConferenciaDeFrete.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            _gridConferenciaDeFrete.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AplicarAjusteConferenciaDeFreteClick(e) {
    obterListaConferenciaFrete();
    var data = { Codigo: e.Codigo.val(), ListaConferenciaFrete: JSON.stringify(_conferenciaDeFrete.ListaConferenciaFrete.list) };
    executarReST("CargaFrete/AplicarAjusteConferenciaDeFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                carregarDadosPedido(0);
                retornoAlteracaoFrete(_cargaAtual, arg);
                Global.fecharModal("divModalConferenciaDeFrete");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ConfirmarConferenciaDeFreteClick(e) {
    var data = { Codigo: e.Codigo.val() };
    executarReST("CargaFrete/ConfirmarConferenciaDeFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                Global.fecharModal("divModalConferenciaDeFrete");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function obterListaConferenciaFrete() {

    var registros = _gridConferenciaDeFrete.GridViewTable().data();
    _conferenciaDeFrete.ListaConferenciaFrete.list = new Array();

    $.each(registros, function (i, registro) {
        var map = new ConferenciaDeFreteMap();
        map.CodigosPedidoXML = registro.CodigosPedidoXML;
        map.Remetente = registro.Remetente;
        map.Destinatario = registro.Destinatario;
        map.ValorLiquido = registro.ValorLiquido;
        _conferenciaDeFrete.ListaConferenciaFrete.list.push(map);
    });
}