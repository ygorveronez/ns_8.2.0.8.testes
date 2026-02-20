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


var _repom;

var Repom = function () {
    this.CodigoFilialRepom = PropertyEntity({ type: types.map, text: Localization.Resources.Transportadores.Transportador.CodigoFilialRepom.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    
    this.PossuiIntegracaoRepom = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

//*******EVENTOS*******
function LoadRepom() {
    _repom = new Repom();
    KoBindings(_repom, "knockoutRepom");
}

function preencherRepom(dadosRepom) {
    if (dadosRepom) {
        _repom.CodigoFilialRepom.val(dadosRepom.CodigoFilialRepom);
    }
}

function LimparRepom() {
    LimparCampos(_repom);
}