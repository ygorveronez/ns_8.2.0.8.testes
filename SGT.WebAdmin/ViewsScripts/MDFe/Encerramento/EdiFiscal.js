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

//*******MAPEAMENTO KNOUCKOUT*******

var _ediFiscal;

var EdiFiscal = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, def: 0 });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, def: 0 });
    this.Lacre = PropertyEntity({ text: "Lacre:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.GerarEDIFiscal = PropertyEntity({ eventClick: gerarEDIFiscal, type: types.event, text: "Gerar EDI Fiscal", idGrid: guid(), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadEdiFiscal() {
    _ediFiscal = new EdiFiscal();
    KoBindings(_ediFiscal, "knockoutEdiFiscal");
}

//*******MÉTODOS*******

function gerarEDIFiscal() {
    executarDownload("Encerramento/DownloadEDIFiscal", {  CodigoMDFe: _ediFiscal.Codigo.val(), Lacre: _ediFiscal.Lacre.val() });
}

function abrirEdiFiscal(dataGrid) {
    _ediFiscal.Codigo.val(dataGrid.Codigo);
    _ediFiscal.Numero.val(dataGrid.Numero);
    Global.abrirModal("divModalEdiFiscal");
}