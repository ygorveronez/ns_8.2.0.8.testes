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
/// <reference path="../../Enumeradores/EnumViaTransporteInternacional.js" />
/// <reference path="../../Enumeradores/EnumIntermediacaoImportacao.js" />
/// <reference path="../../Enumeradores/EnumEstado.js" />
/// <reference path="NFe.js" />

var _viaTransporteInternacional = [
    { text: "Selecione", value: 0 },
    { text: "Marítima", value: EnumViaTransporteInternacional.Maritima },
    { text: "Fluvial", value: EnumViaTransporteInternacional.Fluvial },
    { text: "Lacustre", value: EnumViaTransporteInternacional.Lacustre },
    { text: "Aérea", value: EnumViaTransporteInternacional.Aerea },
    { text: "Postal", value: EnumViaTransporteInternacional.Postal },
    { text: "Ferroviária", value: EnumViaTransporteInternacional.Ferroviaria },
    { text: "Rodoviária", value: EnumViaTransporteInternacional.Rodoviaria },
    { text: "Conduto / Rede Transmissão", value: EnumViaTransporteInternacional.CondutoRedeTransmissao },
    { text: "Meios Próprios", value: EnumViaTransporteInternacional.MeiosProprios },
    { text: "Entrada / Saída ficta", value: EnumViaTransporteInternacional.EntradaSaidaFicta },
    { text: "Courier", value: EnumViaTransporteInternacional.Courier },
    { text: "Handcarry", value: EnumViaTransporteInternacional.Handcarry }
];

var _intermediacaoImportacao = [
    { text: "Selecione", value: 0 },
    { text: "Importação por conta própria", value: EnumIntermediacaoImportacao.Propria },
    { text: "Importação por conta e ordem", value: EnumIntermediacaoImportacao.ContaOrdem },
    { text: "Importação por encomenda", value: EnumIntermediacaoImportacao.Encomenda },
];

var ImpostoII = function (nfe) {

    var instancia = this;

    this.BaseII = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base II:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.DespesaII = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Despesa II:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorII = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor II:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIOFII = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IOF II:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.NumeroDocumentoII = PropertyEntity({ text: "Número Documento: ", required: false, maxlength: 12, enable: ko.observable(true), val: ko.observable("") });
    this.DataRegistroII = PropertyEntity({ getType: typesKnockout.date, text: "Data Registro:", required: false, enable: ko.observable(true), val: ko.observable("") });
    this.LocalDesembaracoII = PropertyEntity({ text: "Local Desembaraço: ", required: false, maxlength: 60, enable: ko.observable(true), val: ko.observable(""), });
    this.EstadoDesembaracoII = PropertyEntity({ val: ko.observable(""), options: EnumEstado.obterOpcoesCadastro(), def: "", enable: ko.observable(true), text: "Estado Desembaraço: ", });
    this.DataDesembaracoII = PropertyEntity({ getType: typesKnockout.date, text: "Data Desembaraço:", required: false, enable: ko.observable(true), val: ko.observable("") });
    this.CNPJAdquirenteII = PropertyEntity({ text: ko.observable("CNPJ do Adquirente: "), required: false, getType: typesKnockout.cnpj, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.ViaTransporteII = PropertyEntity({ val: ko.observable(0), def: 0, options: _viaTransporteInternacional, text: "Via Transporte Internacional:", required: false, enable: ko.observable(true) });
    this.ValorFreteMarinhoII = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Frete Marítimo:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.IntermediacaoII = PropertyEntity({ val: ko.observable(0), def: 0, options: _intermediacaoImportacao, text: "Intermediação Importação:", required: false, enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoII);
    };

    this.DestivarImpostoII = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoII = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };
};