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
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoNDDCargo;

var IntegracaoNDDCargo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URL = PropertyEntity({ text: "*URL: ", maxlength: 200, required: ko.observable(true) });
    this.EnterpriseId = PropertyEntity({ text: "*Enterprise Id: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Token = PropertyEntity({ text: "*Token: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Versao = PropertyEntity({ text: "*Versão: ", maxlength: 15, enable: ko.observable(true), required: ko.observable(true) });
};

//*******EVENTOS*******

function loadConfiguracaoNDDCargo() {
    _integracaoNDDCargo = new IntegracaoNDDCargo();
    KoBindings(_integracaoNDDCargo, "knockoutIntegracaoNDDCargo");
}

//*******MÉTODOS*******

function limparCamposNDDCargo() {
    LimparCampos(_integracaoNDDCargo);
}