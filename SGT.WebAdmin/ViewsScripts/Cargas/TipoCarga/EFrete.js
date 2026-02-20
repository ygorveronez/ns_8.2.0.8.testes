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
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _eFrete;

var eFrete = function () {
    this.TipoDeCargaEFrete = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.TipoDeCargaEFrete.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "", maxlength: 4 });
};

function LoadEFrete() {
    _eFrete = new eFrete();
    KoBindings(_eFrete, "knockoutEFrete");

    _tipoCarga.TipoDeCargaEFrete = _eFrete.TipoDeCargaEFrete;
}

function limparCamposEFrete() {
    LimparCampos(_eFrete);
}