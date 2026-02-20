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
/// <reference path="GrupoPessoas.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _documentoDestinado;

//*******MAPEAMENTO KNOUCKOUT*******

var DocumentoDestinado = function () {
    this.NaoImportarDocumentosDestinadosTransporte = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoImportarDocumentosTransporte, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.NaoImportarDocumentosDestinadosTransporte.val.subscribe(function (novoValor) {
        _grupoPessoas.NaoImportarDocumentosDestinadosTransporte.val(novoValor);
    });
}

//*******EVENTOS*******

function loadDocumentoDestinado() {

    _documentoDestinado = new DocumentoDestinado();
    KoBindings(_documentoDestinado, "knockoutDocumentoDestinado");

}