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

var ImpostoDIFAL = function (nfe) {

    var instancia = this;

    this.BaseICMSDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ICMS UF Destino:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMSDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS UF Destino:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMSInterno = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS Interestadual:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualPartilha = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Partilha:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS UF Destino:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSRemetente = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Remetente:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BaseFCPDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base FCP UF Destino:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualFCP = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% FCP:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorFCP = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor FCP:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoDIFAL);

        var dataAtual = new Date();
        var yyyy = dataAtual.getFullYear().toString();
        if (yyyy == "2016")
            instancia.PercentualPartilha.val("40,00");
        else if (yyyy == "2017")
            instancia.PercentualPartilha.val("60,00");
        else if (yyyy == "2018")
            instancia.PercentualPartilha.val("80,00");
        else
            instancia.PercentualPartilha.val("100,00");
    }

    this.DestivarImpostoDIFAL = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    }

    this.HabilitarImpostoDIFAL = function () {
        HabilitarCamposInstanciasNFe(instancia);
    }

    this.CalcularImpostoFCPInstancia = function () {
        CalcularImpostoFCP(instancia);
    }

    this.CalcularImpostoDIFALInstancia = function () {
        CalcularImpostoDIFAL(instancia);
    }
}

function CalcularImpostoFCP(instancia) {
    var baseFCP = Globalize.parseFloat(instancia.BaseFCPDestino.val());
    var percentualFCP = Globalize.parseFloat(instancia.PercentualFCP.val());

    if (baseFCP > 0 && percentualFCP > 0) {
        var valorFCP = baseFCP * (percentualFCP / 100);
        instancia.ValorFCP.val(Globalize.format(valorFCP, "n2"));
    } else
        instancia.ValorFCP.val(Globalize.format(0, "n2"));
}

function CalcularImpostoDIFAL(instancia) {
    var baseICMSDestino = Globalize.parseFloat(instancia.BaseICMSDestino.val());
    var aliquotaICMSDestino = Globalize.parseFloat(instancia.AliquotaICMSDestino.val());
    var aliquotaICMSInterestadual = Globalize.parseFloat(instancia.AliquotaICMSInterno.val());
    var percentualPartilha = Globalize.parseFloat(instancia.PercentualPartilha.val());

    if ((baseICMSDestino > 0 && aliquotaICMSDestino > 0 && aliquotaICMSInterestadual > 0 && percentualPartilha > 0) && (aliquotaICMSDestino >= aliquotaICMSInterestadual)) {
        var difal = 0;
        difal = baseICMSDestino * ((aliquotaICMSDestino - aliquotaICMSInterestadual) / 100);

        var valorICMSDestino = 0;
        var valorICMSRemetente = 0;

        valorICMSRemetente = difal * ((percentualPartilha - 100) / 100);
        valorICMSDestino = difal * (percentualPartilha / 100);

        instancia.ValorICMSDestino.val(Globalize.format(valorICMSDestino, "n2"));
        instancia.ValorICMSRemetente.val(Globalize.format(valorICMSRemetente, "n2"));
    } else {
        instancia.ValorICMSDestino.val(Globalize.format(0, "n2"));
        instancia.ValorICMSRemetente.val(Globalize.format(0, "n2"));
    }
}