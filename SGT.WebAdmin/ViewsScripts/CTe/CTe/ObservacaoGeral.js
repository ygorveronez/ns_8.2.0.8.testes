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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CTe.js" />

var ObservacaoGeral = function (cte) {

    var instancia = this;

    this.ObservacaoGeral = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.CTes.CTe.ObservacaoGeral, enable: ko.observable(true), maxlength: 2000 });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutObservacao);
    }


    this.DestivarObservacaoGeral = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    }
}

