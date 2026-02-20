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
/// <reference path="../../Enumeradores/EnumCSTPISCOFINS.js" />
/// <reference path="NFe.js" />

var _cstCOFINSSaida = [
    { text: "Selecione", value: 0 },
    { text: "01 - Operação Tributável com Alíquota Básica", value: EnumCSTPISCOFINS.CST01 },
    { text: "02 - Operação Tributável com Alíquota Diferenciada", value: EnumCSTPISCOFINS.CST02 },
    { text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto", value: EnumCSTPISCOFINS.CST03 },
    { text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero", value: EnumCSTPISCOFINS.CST04 },
    { text: "05 - Operação Tributável por Substituição Tributária", value: EnumCSTPISCOFINS.CST05 },
    { text: "06 - Operação Tributável a Alíquota Zero", value: EnumCSTPISCOFINS.CST06 },
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 },
    { text: "49 - Outras Operações de Saída", value: EnumCSTPISCOFINS.CST49 },
    { text: "99 - Outras Operações", value: EnumCSTPISCOFINS.CST99 }
];

var _cstCOFINSEntrada = [
    { text: "Selecione", value: 0 },
    { text: "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST50 },
    { text: "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST51 },
    { text: "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação", value: EnumCSTPISCOFINS.CST52 },
    { text: "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno", value: EnumCSTPISCOFINS.CST53 },
    { text: "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST54 },
    { text: "55 - Operação com Direito a Crédito - Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST55 },
    { text: "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação", value: EnumCSTPISCOFINS.CST56 },
    { text: "60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST60 },
    { text: "61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST61 },
    { text: "62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação", value: EnumCSTPISCOFINS.CST62 },
    { text: "63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno", value: EnumCSTPISCOFINS.CST63 },
    { text: "64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST64 },
    { text: "65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST65 },
    { text: "66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação", value: EnumCSTPISCOFINS.CST66 },
    { text: "67 - Crédito Presumido - Outras Operações", value: EnumCSTPISCOFINS.CST67 },
    { text: "70 - Operação de Aquisição sem Direito a Crédito", value: EnumCSTPISCOFINS.CST70 },
    { text: "71 - Operação de Aquisição com Isenção", value: EnumCSTPISCOFINS.CST71 },
    { text: "72 - Operação de Aquisição com Suspensão", value: EnumCSTPISCOFINS.CST72 },
    { text: "73 - Operação de Aquisição a Alíquota Zero", value: EnumCSTPISCOFINS.CST73 },
    { text: "74 - Operação de Aquisição sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST74 },
    { text: "75 - Operação de Aquisição por Substituição Tributária", value: EnumCSTPISCOFINS.CST75 },
    { text: "98 - Outras Operações de Entrada", value: EnumCSTPISCOFINS.CST98 },
    { text: "99 - Outras Operações", value: EnumCSTPISCOFINS.CST99 }
];

var _cstCOFINSTodos = [
    { text: "Selecione", value: 0 },
    { text: "01 - Operação Tributável com Alíquota Básica", value: EnumCSTPISCOFINS.CST01 },
    { text: "02 - Operação Tributável com Alíquota Diferenciada", value: EnumCSTPISCOFINS.CST02 },
    { text: "03 - Operação Tributável com Alíquota por Unidade de Medida de Produto", value: EnumCSTPISCOFINS.CST03 },
    { text: "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero", value: EnumCSTPISCOFINS.CST04 },
    { text: "05 - Operação Tributável por Substituição Tributária", value: EnumCSTPISCOFINS.CST05 },
    { text: "06 - Operação Tributável a Alíquota Zero", value: EnumCSTPISCOFINS.CST06 },
    { text: "07 - Operação Isenta da Contribuição", value: EnumCSTPISCOFINS.CST07 },
    { text: "08 - Operação sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST08 },
    { text: "09 - Operação com Suspensão da Contribuição", value: EnumCSTPISCOFINS.CST09 },
    { text: "49 - Outras Operações de Saída", value: EnumCSTPISCOFINS.CST49 },
    { text: "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST50 },
    { text: "51 - Operação com Direito a Crédito – Vinculada Exclusivamente a Receita Não Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST51 },
    { text: "52 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita de Exportação", value: EnumCSTPISCOFINS.CST52 },
    { text: "53 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno", value: EnumCSTPISCOFINS.CST53 },
    { text: "54 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST54 },
    { text: "55 - Operação com Direito a Crédito - Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST55 },
    { text: "56 - Operação com Direito a Crédito - Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação", value: EnumCSTPISCOFINS.CST56 },
    { text: "60 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST60 },
    { text: "61 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita Não-Tributada no Mercado Interno", value: EnumCSTPISCOFINS.CST61 },
    { text: "62 - Crédito Presumido - Operação de Aquisição Vinculada Exclusivamente a Receita de Exportação", value: EnumCSTPISCOFINS.CST62 },
    { text: "63 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno", value: EnumCSTPISCOFINS.CST63 },
    { text: "64 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST64 },
    { text: "65 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Não-Tributadas no Mercado Interno e de Exportação", value: EnumCSTPISCOFINS.CST65 },
    { text: "66 - Crédito Presumido - Operação de Aquisição Vinculada a Receitas Tributadas e Não-Tributadas no Mercado Interno, e de Exportação", value: EnumCSTPISCOFINS.CST66 },
    { text: "67 - Crédito Presumido - Outras Operações", value: EnumCSTPISCOFINS.CST67 },
    { text: "70 - Operação de Aquisição sem Direito a Crédito", value: EnumCSTPISCOFINS.CST70 },
    { text: "71 - Operação de Aquisição com Isenção", value: EnumCSTPISCOFINS.CST71 },
    { text: "72 - Operação de Aquisição com Suspensão", value: EnumCSTPISCOFINS.CST72 },
    { text: "73 - Operação de Aquisição a Alíquota Zero", value: EnumCSTPISCOFINS.CST73 },
    { text: "74 - Operação de Aquisição sem Incidência da Contribuição", value: EnumCSTPISCOFINS.CST74 },
    { text: "75 - Operação de Aquisição por Substituição Tributária", value: EnumCSTPISCOFINS.CST75 },
    { text: "98 - Outras Operações de Entrada", value: EnumCSTPISCOFINS.CST98 },
    { text: "99 - Outras Operações", value: EnumCSTPISCOFINS.CST99 }
];

var ImpostoCOFINS = function (nfe) {

    var instancia = this;

    this.CSTCOFINS = PropertyEntity({ val: ko.observable(0), def: 0, options: ko.observable(_cstCOFINSSaida), text: "*CST COFINS:", required: false, enable: ko.observable(true), eventChange: function () { instancia.CSTCOFINSChange(); } });
    this.BaseCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base COFINS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ReducaoBaseCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Redução BC COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoCOFINS);
    };

    this.CSTCOFINSChange = function () {
        if (instancia.CSTCOFINS.val() > 0) {
            instancia.BaseCOFINS.enable(true);
            instancia.ReducaoBaseCOFINS.enable(true);
            instancia.AliquotaCOFINS.enable(true);
            instancia.ValorCOFINS.enable(true);
        } else {
            instancia.BaseCOFINS.enable(false);
            instancia.ReducaoBaseCOFINS.enable(false);
            instancia.AliquotaCOFINS.enable(false);
            instancia.ValorCOFINS.enable(false);
            LimparCampos(instancia);
        }
    };

    this.DestivarImpostoCOFINS = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoCOFINS = function () {
        HabilitarCamposInstanciasNFe(instancia);
        instancia.CSTCOFINSChange();
    };

    this.CalcularImpostoCOFINSInstancia = function () {
        var base = Globalize.parseFloat(instancia.BaseCOFINS.val());
        var reducaoBase = Globalize.parseFloat(instancia.ReducaoBaseCOFINS.val());
        var aliquota = Globalize.parseFloat(instancia.AliquotaCOFINS.val());

        var valorTotalItem = Globalize.parseFloat(nfe.ProdutoServico.ValorTotalItem.val());
        if (base > 0 && reducaoBase > 0 && base < valorTotalItem)
            base = valorTotalItem;

        if (base > 0 && aliquota > 0) {
            if (reducaoBase > 0)
                base = (base - (base * (reducaoBase / 100)));
            var valorCOFINS = base * (aliquota / 100);

            instancia.BaseCOFINS.val(Globalize.format(base, "n2"));
            instancia.ValorCOFINS.val(Globalize.format(valorCOFINS, "n2"));
        } else
            instancia.ValorCOFINS.val(Globalize.format(0, "n2"));
    };
};

function CalcularImpostoCOFINS(instancia) {
    instancia.CalcularImpostoCOFINSInstancia();
}