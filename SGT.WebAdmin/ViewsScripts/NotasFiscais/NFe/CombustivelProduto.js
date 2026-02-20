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

var CombustivelProduto = function (nfe) {

    var instancia = this;

    this.CodigoANP = PropertyEntity({ text: "Código ANP:", getType: typesKnockout.string, maxlength: 9, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualGLP = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Percentual GLP:", getType: typesKnockout.decimal, maxlength: 8, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.PercentualGNN = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Percentual GNN:", getType: typesKnockout.decimal, maxlength: 8, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.PercentualGNI = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Percentual GNI:", getType: typesKnockout.decimal, maxlength: 8, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorPartidaANP = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Partida:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualOrigemComb = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Percentual Origem:", getType: typesKnockout.decimal, maxlength: 8, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.PercentualMisturaBiodiesel = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Percentual Mistura Biodiesel:", getType: typesKnockout.decimal, maxlength: 8, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutCombustivelProduto);
        instancia.PercentualOrigemComb.val("0,0000");
        instancia.PercentualMisturaBiodiesel.val("0,0000");
    }

    this.DestivarCombustivelProduto = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    }

    this.HabilitarCombustivelProduto = function () {
        HabilitarCamposInstanciasNFe(instancia);
    }
}