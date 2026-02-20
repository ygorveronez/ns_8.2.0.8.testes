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
/// <reference path="../../Enumeradores/EnumMotivoDesoneracaoICMS.js" />
/// <reference path="NFe.js" />

var _motivoDesoneracao = [
    { text: "Selecione", value: 0 },
    { text: "Táxi", value: EnumMotivoDesoneracaoICMS.Taxi },
    { text: "Deficiente Físico", value: EnumMotivoDesoneracaoICMS.DeficienteFisico },
    { text: "Produtor Agropecuário", value: EnumMotivoDesoneracaoICMS.ProdutorAgropecuario },
    { text: "Frotista/Locadora", value: EnumMotivoDesoneracaoICMS.FrotistaLocadora },
    { text: "Diplomático/Consular", value: EnumMotivoDesoneracaoICMS.DiplomaticoConsular },
    { text: "Utilitários e Motocicletas da Amazônia Ocidental e Áreas de Livre Comércio", value: EnumMotivoDesoneracaoICMS.UtilitariosMotocicleta },
    { text: "SUFRAMA", value: EnumMotivoDesoneracaoICMS.SUFRAMA },
    { text: "Venda a Órgão Público", value: EnumMotivoDesoneracaoICMS.VendaOrgaoPublico },
    { text: "Outros", value: EnumMotivoDesoneracaoICMS.Outros },
    { text: "Deficiente Condutor", value: EnumMotivoDesoneracaoICMS.DeficienteCondutor },
    { text: "Deficiente Não Condutor", value: EnumMotivoDesoneracaoICMS.DeficienteNaoCondutor },
    { text: "Órgão de fomento e desenvolvimento agropecuário", value: EnumMotivoDesoneracaoICMS.FomentoEDesenvolvimentoAgropecuario },
    { text: "Olimpíadas Rio 2016", value: EnumMotivoDesoneracaoICMS.OlimpiadasRio2016 },
    { text: "Solicitado pelo Fisco", value: EnumMotivoDesoneracaoICMS.SolicitadoPeloFisco }
];

var ImpostoICMSDesonerado = function (nfe) {

    var instancia = this;

    this.ValorICMSDesonerado = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Desonerado:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.MotivoICMSDesonerado = PropertyEntity({ val: ko.observable(0), def: 0, options: _motivoDesoneracao, text: "Motivo da Desoneração:", required: false, enable: ko.observable(true) });

    this.ValorICMSOperacao = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Operação:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMSOperacao = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Diferimento:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSDeferido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Deferido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoICMSDesonerado);
    };

    this.DestivarImpostoICMSDesonerado = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoICMSDesonerado = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };
};