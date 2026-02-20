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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="MovimentacaoDePlacas.js" />
/// <reference path="Reboques.js" />
/// <reference path="../../Consultas/Motorista.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridMotorista;
var _pesquisaMotorista;

var PesquisaMotorista = function () {
    this.Placa = PropertyEntity({ text: "Placa: " });
    this.Nome = PropertyEntity({ text: "Nome: ", getType: typesKnockout.string });
    this.CPF = PropertyEntity({ text: "CPF: ", maxlength: 14, getType: typesKnockout.cpf, val: ko.observable("") });
    this.SomentePedentes = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Retornar somente motoristas disponíveis?", def: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotorista.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });    
}


//*******EVENTOS*******

function loadMotoristas(callback) {
    _pesquisaMotorista = new PesquisaMotorista();
    KoBindings(_pesquisaMotorista, "knockoutPesquisaMotoristas", false, _pesquisaMotorista.Pesquisar.id);
    $("#" + _pesquisaMotorista.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _pesquisaMotorista.SomentePedentes.id).click(function () {
        if (_gridMotorista != null)
            _gridMotorista.CarregarGrid();
    });

    buscarMotoristas(callback);
}


//*******MÉTODOS*******

function consultarMotoristas(e) {
    if (_gridMotorista != null && _pesquisaMotorista.CPF.val() != "")
        _gridMotorista.CarregarGrid();
}

function verificaPlacaMotorista(e) {
    if ($("#" + _pesquisaMotorista.Placa.id).val().replace(/\_/g, "").length < 7 && $("#" + _pesquisaMotorista.Placa.id).val().replace(/\_/g, "").length > 0) {
        _pesquisaMotorista.Placa.val("");
        $("#" + _pesquisaMotorista.Placa.id).val("");
    }
}

function buscarMotoristas(callback) {
    _gridMotorista = new GridView(_pesquisaMotorista.Pesquisar.idGrid, "Motorista/PesquisaMotoristaMovimentacaoDePlacas", _pesquisaMotorista, null, null, null, null, false, true);
    _gridMotorista.CarregarGrid(callback);
}

