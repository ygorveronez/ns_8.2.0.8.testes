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

var _configuracaoSenig;

var ConfiguracaoSenig = function () {
    this.AverbaComoEmbarcador = PropertyEntity({ text: "Averba Como Embarcador", val: ko.observable(false), def: false });
}

//*******EVENTOS*******

function loadConfiguracaoSenig() {
    _configuracaoSenig = new ConfiguracaoSenig();
    KoBindings(_configuracaoSenig, "knockoutSenig");
}
