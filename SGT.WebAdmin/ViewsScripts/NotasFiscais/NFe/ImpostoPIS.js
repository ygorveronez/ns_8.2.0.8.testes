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

var _cstPISSaida = [
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

var _cstPISEntrada = [
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

var _cstPISTodos = [
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

var ImpostoPIS = function (nfe, impostoCOFINS, impostoIPI) {

    var instancia = this;

    this.CSTPIS = PropertyEntity({ val: ko.observable(0), def: 0, options: ko.observable(_cstPISSaida), text: "*CST PIS:", required: false, enable: ko.observable(true), eventChange: function () { instancia.CSTPISChange(); } });
    this.BasePIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base PIS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ReducaoBasePIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Redução BC PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaPIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorPIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoPIS);
    };

    this.CSTPISChange = function () {
        if (instancia.CSTPIS.val() > 0) {
            instancia.BasePIS.enable(true);
            instancia.ReducaoBasePIS.enable(true);
            instancia.AliquotaPIS.enable(true);
            instancia.ValorPIS.enable(true);
        } else {
            instancia.BasePIS.enable(false);
            instancia.ReducaoBasePIS.enable(false);
            instancia.AliquotaPIS.enable(false);
            instancia.ValorPIS.enable(false);
            LimparCampos(instancia);
        }
    };

    this.DestivarImpostoPIS = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoPIS = function () {
        HabilitarCamposInstanciasNFe(instancia);
        instancia.CSTPISChange();
    };

    this.TipoEmissaoChange = function () {
        instancia.CSTPIS.options(_cstPISTodos);
        impostoCOFINS.CSTCOFINS.options(_cstCOFINSTodos);
        impostoIPI.CSTIPI.options(_cstIPITodos);
    };

    this.CalcularImpostoPISInstancia = function () {
        var basePIS = Globalize.parseFloat(instancia.BasePIS.val());
        var reducaoBasePIS = Globalize.parseFloat(instancia.ReducaoBasePIS.val());
        var aliquotaPIS = Globalize.parseFloat(instancia.AliquotaPIS.val());

        var valorTotalItem = Globalize.parseFloat(nfe.ProdutoServico.ValorTotalItem.val());
        if (basePIS > 0 && reducaoBasePIS > 0 && basePIS < valorTotalItem)
            basePIS = valorTotalItem;

        if (basePIS > 0 && aliquotaPIS > 0) {
            if (reducaoBasePIS > 0)
                basePIS = (basePIS - (basePIS * (reducaoBasePIS / 100)));
            var valorPIS = basePIS * (aliquotaPIS / 100);

            instancia.BasePIS.val(Globalize.format(basePIS, "n2"));
            instancia.ValorPIS.val(Globalize.format(valorPIS, "n2"));
        }
        else
            instancia.ValorPIS.val(Globalize.format(0, "n2"));
    };
};

function CalcularImpostoPIS(instancia) {
    instancia.CalcularImpostoPISInstancia();
}