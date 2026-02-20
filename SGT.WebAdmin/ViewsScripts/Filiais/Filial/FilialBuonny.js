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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _filialBuonny;

var FilialBuonny = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CNPJCliente = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CNPJCliente.getFieldDescription(), maxlength: 14, visible: ko.observable(true), required: false });
    this.Token = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Token.getFieldDescription(), maxlength: 50, visible: ko.observable(true), required: false });
}

//*******EVENTOS*******

function loadFilialBuonny() {
    _filialBuonny = new FilialBuonny();
    KoBindings(_filialBuonny, "knockoutIntegracaoBuonny");
}

//*******MÉTODOS*******

function limparCamposFilialBuonny() {
    LimparCampos(_filialBuonny);
}