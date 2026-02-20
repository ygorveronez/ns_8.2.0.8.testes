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
/// <reference path="NFe.js" />

var LotesProduto = function (nfe) {

    var instancia = this;

    this.NumeroLote = PropertyEntity({ text: "Número Lote:", getType: typesKnockout.string, maxlength: 20, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.QuantidadeLote = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Quantidade Lote:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.DataFabricacao = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data Fabrição:", required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataValidade = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data Validade:", required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoAgregacao = PropertyEntity({ text: "Código Agregação:", getType: typesKnockout.string, maxlength: 20, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutLotesProduto);
    }

    this.DestivarLotesProduto = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    }

    this.HabilitarLotesProduto = function () {
        HabilitarCamposInstanciasNFe(instancia);
    }
}