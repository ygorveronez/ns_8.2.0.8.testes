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

var _repomFrete;

var RepomFrete = function () {
    // Codigo da entidade
    this.CodigoFilialRepom = PropertyEntity({ text: Localization.Resources.Logistica.CentroCustoViagem.CodigoFilialRepom, required: false, getType: typesKnockout.string, val: ko.observable(""), maxlengh: 15 });
}

//*******EVENTOS*******
function loadCentroCustoViagemRepomFrete() {
    //-- Knouckout
    // Instancia objeto principal
    _repomFrete = new RepomFrete();
    KoBindings(_repomFrete, "knockoutRepomFrete");

    if (_CONFIGURACAO_TMS.possuiIntegracaoRepomFrete) {
        $("#liRepomFrete").show();
    }
    else {
        $("#liRepomFrete").hide();
    }
}


//*******MÉTODOS*******
