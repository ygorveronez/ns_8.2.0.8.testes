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
/// <reference path="../../Enumeradores/EnumCSTIPI.js" />
/// <reference path="NFe.js" />

var _cstIPISaida = [
    { text: "Selecione", value: 0 },
    { text: "50 - Saída tributada", value: EnumCSTIPI.CST50 },
    { text: "51 - Saída tributada com alíquota zero", value: EnumCSTIPI.CST51 },
    { text: "52 - Saída isent", value: EnumCSTIPI.CST52 },
    { text: "53 - Saída não-tributada", value: EnumCSTIPI.CST53 },
    { text: "54 - Saída imune", value: EnumCSTIPI.CST54 },
    { text: "55 - Saída com suspensão", value: EnumCSTIPI.CST55 },
    { text: "99 - Outras saídas", value: EnumCSTIPI.CST99 }
];

var _cstIPIEntrada = [
    { text: "Selecione", value: 0 },
    { text: "00 - Entrada com recuperação de crédito", value: EnumCSTIPI.CST00 },
    { text: "01 - Entrada tributada com alíquota zero", value: EnumCSTIPI.CST01 },
    { text: "02 - Entrada isenta", value: EnumCSTIPI.CST02 },
    { text: "03 - Entrada não-tributada", value: EnumCSTIPI.CST03 },
    { text: "04 - Entrada imune", value: EnumCSTIPI.CST04 },
    { text: "05 - Entrada com suspenção", value: EnumCSTIPI.CST05 },
    { text: "49 - Outras entradas", value: EnumCSTIPI.CST49 }
];

var _cstIPITodos = [
    { text: "Selecione", value: 0 },
    { text: "00 - Entrada com recuperação de crédito", value: EnumCSTIPI.CST00 },
    { text: "01 - Entrada tributada com alíquota zero", value: EnumCSTIPI.CST01 },
    { text: "02 - Entrada isenta", value: EnumCSTIPI.CST02 },
    { text: "03 - Entrada não-tributada", value: EnumCSTIPI.CST03 },
    { text: "04 - Entrada imune", value: EnumCSTIPI.CST04 },
    { text: "05 - Entrada com suspenção", value: EnumCSTIPI.CST05 },
    { text: "49 - Outras entradas", value: EnumCSTIPI.CST49 },
    { text: "50 - Saída tributada", value: EnumCSTIPI.CST50 },
    { text: "51 - Saída tributada com alíquota zero", value: EnumCSTIPI.CST51 },
    { text: "52 - Saída isent", value: EnumCSTIPI.CST52 },
    { text: "53 - Saída não-tributada", value: EnumCSTIPI.CST53 },
    { text: "54 - Saída imune", value: EnumCSTIPI.CST54 },
    { text: "55 - Saída com suspensão", value: EnumCSTIPI.CST55 },
    { text: "99 - Outras saídas", value: EnumCSTIPI.CST99 }
];

var ImpostoIPI = function (nfe) {

    var instancia = this;

    this.CSTIPI = PropertyEntity({ val: ko.observable(0), def: 0, options: ko.observable(_cstIPISaida), text: "CST IPI:", required: false, enable: ko.observable(true), eventChange: function () { instancia.CSTIPIChange(); } });
    this.BaseIPI = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base IPI:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ReducaoBaseIPI = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Redução BC IPI:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaIPI = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota IPI:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIPI = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IPI:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.PercentualIPIDevolvido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Percentual IPI Devolvido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIPIDevolvido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IPI Devolvido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoIPI);
    };

    this.CSTIPIChange = function () {
        if (instancia.CSTIPI.val() > 0) {
            instancia.BaseIPI.enable(true);
            instancia.ReducaoBaseIPI.enable(true);
            instancia.AliquotaIPI.enable(true);
            instancia.ValorIPI.enable(true);
            instancia.PercentualIPIDevolvido.enable(true);
            instancia.ValorIPIDevolvido.enable(true);
        } else {
            instancia.BaseIPI.enable(false);
            instancia.ReducaoBaseIPI.enable(false);
            instancia.AliquotaIPI.enable(false);
            instancia.ValorIPI.enable(false);
            instancia.PercentualIPIDevolvido.enable(false);
            instancia.ValorIPIDevolvido.enable(false);
            LimparCampos(instancia);
        }
    };

    this.DestivarImpostoIPI = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoIPI = function () {
        HabilitarCamposInstanciasNFe(instancia);
        instancia.CSTIPIChange();
    };

    this.CalcularImpostoIPIInstancia = function () {
        var base = Globalize.parseFloat(instancia.BaseIPI.val());
        var reducaoBase = Globalize.parseFloat(instancia.ReducaoBaseIPI.val());
        var aliquota = Globalize.parseFloat(instancia.AliquotaIPI.val());

        var valorTotalItem = Globalize.parseFloat(nfe.ProdutoServico.ValorTotalItem.val());
        if (base > 0 && reducaoBase > 0 && base < valorTotalItem)
            base = valorTotalItem;

        if (base > 0 && aliquota > 0) {
            if (reducaoBase > 0)
                base = (base - (base * (reducaoBase / 100)));
            var valorIPI = base * (aliquota / 100);

            instancia.BaseIPI.val(Globalize.format(base, "n2"));
            instancia.ValorIPI.val(Globalize.format(valorIPI, "n2"));
        } else
            instancia.ValorIPI.val(Globalize.format(0, "n2"));
    };
};

function CalcularImpostoIPI(instancia) {
    instancia.CalcularImpostoIPIInstancia();
}