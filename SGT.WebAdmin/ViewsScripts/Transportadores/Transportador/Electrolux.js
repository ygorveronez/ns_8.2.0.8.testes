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


var _electrolux;

var Electrolux = function () {
    this.IdentificadorTransportador = PropertyEntity({ type: types.map, text: Localization.Resources.Transportadores.Transportador.IdentificadorTransportador.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    
    this.PossuiIntegracaoElectrolux = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

//*******EVENTOS*******
function LoadElectrolux() {
    _electrolux = new Electrolux();
    KoBindings(_electrolux, "knockoutElectrolux");
}

function preencherElectrolux(dadosElectrolux) {
    if (dadosElectrolux) {
        _electrolux.IdentificadorTransportador.val(dadosElectrolux.IdentificadorTransportador);
    }
}

function LimparElectrolux() {
    LimparCampos(_electrolux);
}