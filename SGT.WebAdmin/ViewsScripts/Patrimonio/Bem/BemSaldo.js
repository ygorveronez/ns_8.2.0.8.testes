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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Bem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _bemSaldo;

var BemSaldo = function () {
    this.DataImplantacao = PropertyEntity({ text: "*Data Implantação:", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataEntradaTransferencia = PropertyEntity({ text: "Data Entrada Transfêrencia:", getType: typesKnockout.date });
    this.DataBaixa = PropertyEntity({ text: "Data Baixa:", getType: typesKnockout.date });
    this.DataAlocado = PropertyEntity({ text: "Data Alocação:", getType: typesKnockout.date });

    this.VidaUtil = PropertyEntity({ text: "Vida Útil (Anos):", getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.ValorBem = PropertyEntity({ text: "*Valor do Patrimônio:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 4, allowZero: false, allowNegative: false }, required: ko.observable(true) });
    this.PercentualResidual = PropertyEntity({ text: "Percentual Residual:", getType: typesKnockout.decimal, maxlength: 6, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.ValorResidual = PropertyEntity({ text: "Valor Residual:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.ValorDepreciar = PropertyEntity({ text: "Valor Depreciar:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.DepreciacaoAcumulada = PropertyEntity({ text: "Depreciação Acumulada:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(false) });
    this.PercentualDepreciacao = PropertyEntity({ text: "Percentual Depreciação:", getType: typesKnockout.decimal, maxlength: 6, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.VidaUtilTaxaDepreciacao = PropertyEntity({ text: "Taxa Depreciação Vida Útil (Ano):", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.FuncionarioAlocado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Alocado:", idBtnSearch: guid(), required: ko.observable(false) });
};

//*******EVENTOS*******

function loadBemSaldo() {
    _bemSaldo = new BemSaldo();
    KoBindings(_bemSaldo, "knockoutSaldoBem");

    new BuscarFuncionario(_bemSaldo.FuncionarioAlocado);
}

//*******MÉTODOS*******

function limparCamposBemSaldo() {
    LimparCampos(_bemSaldo);
}

function validaCamposObrigatoriosBemSaldo() {
    return ValidarCamposObrigatorios(_bemSaldo);
}
