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
/// <reference path="NFe.js" />

var ImpostoICMSST = function (nfe, impostoIPI, listaProdutoServico) {

    var instancia = this;

    this.BaseICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ICMS ST:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ReducaoBaseICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Redução MVA ICMS ST:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualMVA = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% MVA", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS ST:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaInterestadual = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota Interestadual:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS ST:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BaseFCPICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base FCP ICMS ST:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualFCPICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Percentual FCP ICMS ST:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorFCPICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor FCP ICMS ST:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaFCPICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota FCP ICMS ST:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });//Não utilizada, AliquotaICMSSTRetido seta na pST

    this.BCICMSSTRetido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ICMS ST Retido:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMSSTRetido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS ST Retido (Suporta Consumidor Final):", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSSTSubstituto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS próprio do Substituto:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSSTRetido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS ST Retido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BCICMSSTDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ICMS ST Destino:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSSTDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS ST Destino:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoICMSST);
    };

    this.CalcularICMSST = function () {
        var valorIPI = Globalize.parseFloat(impostoIPI.ValorIPI.val());
        var valorFrete = Globalize.parseFloat(listaProdutoServico.Frete.val());
        var valorSeguro = Globalize.parseFloat(listaProdutoServico.Seguro.val());
        var valorOutras = Globalize.parseFloat(listaProdutoServico.Outras.val());
        var valorDesconto = Globalize.parseFloat(listaProdutoServico.Desconto.val());
        var valorTotalItem = Globalize.parseFloat(nfe.ProdutoServico.ValorTotalItem.val());
        var base = Globalize.parseFloat(instancia.BaseICMSST.val());
        var reducaoBase = Globalize.parseFloat(instancia.ReducaoBaseICMSST.val());
        var mva = Globalize.parseFloat(instancia.PercentualMVA.val());
        var aliquotaInterna = Globalize.parseFloat(instancia.AliquotaICMSST.val());
        var aliquotaInterestadual = Globalize.parseFloat(instancia.AliquotaInterestadual.val());

        if (base > 0 && aliquotaInterna > 0 && mva > 0 && aliquotaInterestadual > 0 && valorTotalItem > 0) {
            var valorICMSInterestadual = 0;
            valorICMSInterestadual = (valorTotalItem + valorFrete + valorSeguro + valorOutras - valorDesconto);
            valorICMSInterestadual = valorICMSInterestadual * (aliquotaInterestadual / 100);

            if (reducaoBase > 0)
                mva = mva * (reducaoBase / 100);
            base = (valorTotalItem + valorIPI + valorFrete + valorSeguro + valorOutras - valorDesconto);

            var baseICMSST = base * (1 + (mva / 100));
            var valorICMST = (baseICMSST * (aliquotaInterna / 100)) - valorICMSInterestadual;

            instancia.ValorICMSST.val(Globalize.format(valorICMST, "n2"));
            instancia.BaseICMSST.val(Globalize.format(baseICMSST, "n2"));
        }
    };

    this.CalcularImpostoFCPICMSSTInstancia = function () {
        CalcularImpostoFCPICMSST(instancia);
    };

    this.DestivarImpostoICMSST = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoICMSST = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };

    this.BCICMSSTRetido.val.subscribe(function () {
        CalcularImpostoICMSSTRetido(instancia);
    });
    this.AliquotaICMSSTRetido.val.subscribe(function () {
        CalcularImpostoICMSSTRetido(instancia);
    });
};

function CalcularImpostoICMSST(instancia) {
    instancia.CalcularICMSST();
}

function CalcularImpostoFCPICMSST(instancia) {
    var baseFCPICMSST = Globalize.parseFloat(instancia.BaseFCPICMSST.val());
    var percentualFCPICMSST = Globalize.parseFloat(instancia.PercentualFCPICMSST.val());

    if (baseFCPICMSST > 0 && percentualFCPICMSST > 0) {
        var valorFCPICMSST = baseFCPICMSST * (percentualFCPICMSST / 100);
        instancia.ValorFCPICMSST.val(Globalize.format(valorFCPICMSST, "n2"));
    } else
        instancia.ValorFCPICMSST.val(Globalize.format(0, "n2"));
}

function CalcularImpostoICMSSTRetido(instancia) {
    var baseICMSSTRetido = Globalize.parseFloat(instancia.BCICMSSTRetido.val());
    var aliquotaICMSSTRetido = Globalize.parseFloat(instancia.AliquotaICMSSTRetido.val());

    if (baseICMSSTRetido > 0 && aliquotaICMSSTRetido > 0) {
        var valorICMSSTRetido = baseICMSSTRetido * (aliquotaICMSSTRetido / 100);
        instancia.ValorICMSSTRetido.val(Globalize.format(valorICMSSTRetido, "n2"));
    } else
        instancia.ValorICMSSTRetido.val(Globalize.format(0, "n2"));
}