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

var _openTech;

var OpenTech = function () {
    // Codigo da entidade
    this.CodigoTransportadorOpenTech = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.CodigoTransportadorOpenTech, required: false, getType: typesKnockout.int, val: ko.observable("") });

}

//*******EVENTOS*******
function loadCentroCustoViagemOpenTech() {
    //-- Knouckout
    // Instancia objeto principal
    _openTech = new OpenTech();
    KoBindings(_openTech, "knockoutOpenTech");

    if (_CONFIGURACAO_TMS.possuiIntegracaoOpenTech) {
        $("#liOpenTech").show();
    }
    else {
        $("#liOpenTech").hide();
    }
}


//*******MÉTODOS*******
