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
/// <reference path="NFSe.js" />

var Substituicao = function (nfse) {

    var instancia = this;

    this.Numero = PropertyEntity({ getType: typesKnockout.int, text: "Número RPS:", enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: "Série:", enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfse.IdKnockoutSubstituicao);
    }

    this.DestivarObservacao = function () {
        DesabilitarCamposInstanciasNFSe(instancia);
    }
}