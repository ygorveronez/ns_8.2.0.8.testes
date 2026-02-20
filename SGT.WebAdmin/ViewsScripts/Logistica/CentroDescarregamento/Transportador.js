/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorCentroDescarregamento.js" />

var _gridTransportador;
var _transportador;

var _tipoTransportadorCentroDescarregamento = [
    { value: EnumTipoTransportadorCentroDescarregamento.Todos, text: "Todos" },
    { value: EnumTipoTransportadorCentroDescarregamento.TodosComTipoVeiculoCarga, text: "Todos com o tipo de veículo da carga" },
    { value: EnumTipoTransportadorCentroDescarregamento.TodosCentroDescarregamento, text: "Todos do centro de carregamento" },
    { value: EnumTipoTransportadorCentroDescarregamento.TodosCentroDescarregamentoComTipoVeiculoCarga, text: "Todos do centro de carregamento com o tipo de veículo da carga" }
];

var Transportador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Transportador = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarTransportador, idBtnSearch: guid(), visible: ko.observable(true) });

    this.LiberarCargaAutomaticamenteParaTransportadoras = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.LiberarCargasAutomaticamenteTransportadoras, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.TipoTransportadorCentroDescarregamento = PropertyEntity({ val: ko.observable(EnumTipoTransportadorCentroDescarregamento.Todos), options: _tipoTransportadorCentroDescarregamento, text: Localization.Resources.Logistica.CentroDescarregamento.TipoTransportador.getRequiredFieldDescription(), def: EnumTipoTransportadorCentroDescarregamento.Todos });

    this.TipoTransportadorCentroDescarregamento.val.subscribe(function (novoValor) {
        _centroDescarregamento.TipoTransportadorCentroDescarregamento.val(novoValor);
    });

    this.LiberarCargaAutomaticamenteParaTransportadoras.val.subscribe(function (novoValor) {
        _centroDescarregamento.LiberarCargaAutomaticamenteParaTransportadoras.val(novoValor);
    });
}


//*******EVENTOS*******

function LoadTransportador() {

    _transportador = new Transportador();
    KoBindings(_transportador, "knockoutTransportador");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTransportadorClick(_transportador.Transportador, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Logistica.CentroDescarregamento.RazaoSocial, width: "80%" }];

    _gridTransportador = new BasicDataTable(_transportador.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    new BuscarTransportadores(_transportador.Transportador, null, null, true, _gridTransportador);

    _transportador.Transportador.basicTable = _gridTransportador;

    RecarregarGridTransportador();
}

function RecarregarGridTransportador() {
    _gridTransportador.CarregarGrid(_centroDescarregamento.Transportadores.val());
}

function ExcluirTransportadorClick(knoutTransportador, data) {
    var transportadoresGrid = knoutTransportador.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadoresGrid.length; i++) {
        if (data.Codigo == transportadoresGrid[i].Codigo) {
            transportadoresGrid.splice(i, 1);
            break;
        }
    }

    knoutTransportador.basicTable.CarregarGrid(transportadoresGrid);
}

function LimparCamposTransportador() {
    LimparCampos(_transportador);
    _gridTransportador.CarregarGrid(new Array());
}