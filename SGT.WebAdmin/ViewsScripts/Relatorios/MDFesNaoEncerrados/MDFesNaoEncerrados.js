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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />

var _gridMDFesNaoEncerrados;
var _pesquisaMDFesNaoEncerrados;

var PesquisaMDFesNaoEncerrados = function () {

    this.Atualizar = PropertyEntity({
        eventClick: function (e) {
            _gridMDFesNaoEncerrados.CarregarGrid();
        }, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(true)
    });

}

function CarregarMDFesNaoEncerrados() {
    _gridMDFesNaoEncerrados = new GridView(_pesquisaMDFesNaoEncerrados.Atualizar.idGrid, "MDFesNaoEncerrados/Consultar", _pesquisaMDFesNaoEncerrados, null, { column: 0, dir: orderDir.asc }, 10, null);
    _gridMDFesNaoEncerrados.CarregarGrid();
}

function loadMDFesNaoEncerrados() {
    _pesquisaMDFesNaoEncerrados = new PesquisaMDFesNaoEncerrados();
    KoBindings(_pesquisaMDFesNaoEncerrados, "knockoutRelatorio");

    CarregarMDFesNaoEncerrados();
}