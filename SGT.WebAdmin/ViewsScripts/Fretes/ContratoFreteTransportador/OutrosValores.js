/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _outrosValores;

var _descontarValoresOutrasCargas = [
    { text: "Sim", value: true },
    { text: "Não", value: false },
];

var OutrosValores = function () {
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DescontarValoresOutrasCargas = PropertyEntity({ val: ko.observable(true), options: _descontarValoresOutrasCargas, def: true, enable: ko.observable(true), text: "Descontar valores de outras cargas: " });

    this.DiariaVeiculo = PropertyEntity({ text: "Valor Diária Por Veículo:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.QuinzenaVeiculo = PropertyEntity({ text: "Valor Quinzenal Por Veículo:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.QuantidadeMotoristas = PropertyEntity({ text: "Quantidade Motoristas/Manobristas:", type: types.map, getType: typesKnockout.int, enable: ko.observable(true)});
    this.DiariaMotorista = PropertyEntity({ text: "Valor Diária Motoristas/Manobristas:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.QuinzenaMotorista = PropertyEntity({ text: "Valor Quinzenal Motoristas/Manobristas:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorKmExcedente = PropertyEntity({ text: "Valor Km Excedente:", type: types.map, getType: typesKnockout.decimal, enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadOutrosValores() {
    _outrosValores = new OutrosValores();
    KoBindings(_outrosValores, "knockoutOutrosValores");
}

//*******MÉTODOS*******
function EditarOutrosValores(data) {
    LimparCamposOutrosValores();
    _outrosValores.ContratoFreteTransportador.val(_contratoFreteTransportador.Codigo.val());
    PreencherObjetoKnout(_outrosValores, { Data: data });
}

function LimparCamposOutrosValores() {
    LimparCampos(_outrosValores);
}
