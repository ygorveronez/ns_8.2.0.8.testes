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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />
/// <reference path="NFe.js" />

var ImpostoICMS = function (nfe) {

    var instancia = this;

    this.BaseICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ICMS:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ReducaoBaseICMS = PropertyEntity({ def: "0,000000", val: ko.observable("0,000000"), text: "% Redução BC ICMS:", getType: typesKnockout.decimal, maxlength: 10, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 6, allowZero: true } });
    this.AliquotaICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BaseFCPICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base FCP ICMS:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualFCPICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Percentual FCP ICMS:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorFCPICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor FCP ICMS:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BCICMSEfetivo = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ICMS Efetivo:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMSEfetivo = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS Efetivo:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ReducaoBCICMSEfetivo = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Redução BC ICMS Efetivo:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSEfetivo = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Efetivo:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoICMS);
    };

    this.DestivarImpostoICMS = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoICMS = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };

    this.CalcularImpostoICMSInstancia = function () {
        var baseICMS = Globalize.parseFloat(instancia.BaseICMS.val());
        var reducaoICMS = Globalize.parseFloat(instancia.ReducaoBaseICMS.val());
        var aliquotaICMS = Globalize.parseFloat(instancia.AliquotaICMS.val());

        var valorTotalItem = Globalize.parseFloat(nfe.ProdutoServico.ValorTotalItem.val());
        if (baseICMS > 0 && baseICMS < valorTotalItem)
            baseICMS = valorTotalItem;

        if (nfe.NFe.SubtraiDescontoBaseICMS.val()) {
            var descontoItem = Globalize.parseFloat(nfe.ListaProdutoServico.Desconto.val());
            if (descontoItem > 0 && baseICMS > descontoItem)
                baseICMS = baseICMS - descontoItem;
        }

        if (baseICMS > 0 && aliquotaICMS > 0) {
            if (reducaoICMS > 0)
                baseICMS = (baseICMS - (baseICMS * (reducaoICMS / 100)));
            var valorICMS = baseICMS * (aliquotaICMS / 100);

            instancia.BaseICMS.val(Globalize.format(baseICMS, "n2"));
            instancia.ValorICMS.val(Globalize.format(valorICMS, "n2"));
        } else
            instancia.ValorICMS.val(Globalize.format(0, "n2"));
    };
};

function CalcularImpostoICMS(instancia) {
    instancia.CalcularImpostoICMSInstancia();
}

function CalcularImpostoFCPICMS(instancia) {
    var baseFCPICMS = Globalize.parseFloat(instancia.BaseFCPICMS.val());
    var percentualFCPICMS = Globalize.parseFloat(instancia.PercentualFCPICMS.val());

    if (baseFCPICMS > 0 && percentualFCPICMS > 0) {
        var valorFCPICMS = baseFCPICMS * (percentualFCPICMS / 100);
        instancia.ValorFCPICMS.val(Globalize.format(valorFCPICMS, "n2"));
    } else
        instancia.ValorFCPICMS.val(Globalize.format(0, "n2"));
}

function CalcularImpostoICMSEfetivo(instancia) {
    var baseICMSEfetivo = Globalize.parseFloat(instancia.BCICMSEfetivo.val());
    var reducaoICMSEfetivo = Globalize.parseFloat(instancia.ReducaoBCICMSEfetivo.val());
    var aliquotaICMSEfetivo = Globalize.parseFloat(instancia.AliquotaICMSEfetivo.val());

    if (baseICMSEfetivo > 0 && aliquotaICMSEfetivo > 0) {
        var valorICMSEfetivo = 0;
        if (reducaoICMSEfetivo === 0)
            valorICMSEfetivo = baseICMSEfetivo * (aliquotaICMSEfetivo / 100);
        else
            valorICMSEfetivo = (baseICMSEfetivo - (baseICMSEfetivo * (reducaoICMSEfetivo / 100))) * (aliquotaICMSEfetivo / 100);

        instancia.ValorICMSEfetivo.val(Globalize.format(valorICMSEfetivo, "n2"));
    } else
        instancia.ValorICMSEfetivo.val(Globalize.format(0, "n2"));
}