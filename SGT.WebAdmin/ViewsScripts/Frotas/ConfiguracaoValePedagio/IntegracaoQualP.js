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

var _integracaoQualP;

var IntegracaoQualP = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URL = PropertyEntity({ text: "*Url: ", val: ko.observable(""), def: "", maxlength: 400, enable: ko.observable(true), required: true });
    this.Token = PropertyEntity({ text: "*Token: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: true });
    this.Observacao = PropertyEntity({ text: "Observação 1: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.DistanciaMinimaQuadrante = PropertyEntity({ val: ko.observable(1000), getType: typesKnockout.int, enable: ko.observable(true), text: "Distância Mínima quadrante polilinha", def: false });
};

//*******EVENTOS*******

function loadConfiguracaoQualP() {
    _integracaoQualP = new IntegracaoQualP();
    KoBindings(_integracaoQualP, "knockoutIntegracaoQualP");
}
