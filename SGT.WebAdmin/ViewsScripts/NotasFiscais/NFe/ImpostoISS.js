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
/// <reference path="../../Enumeradores/EnumExigibilidadeISS.js" />
/// <reference path="NFe.js" />

var _exigibilidadeISS = [
    { text: "Selecione", value: 0 },
    { text: "Exigível", value: EnumExigibilidadeISS.Exigivel },
    { text: "Não incidência", value: EnumExigibilidadeISS.NaoInicidencia },
    { text: "Isenção", value: EnumExigibilidadeISS.Isencao },
    { text: "Exportação", value: EnumExigibilidadeISS.Exportacao },
    { text: "Imunidade", value: EnumExigibilidadeISS.Imunidade },
    { text: "Suspensa por Decisão Judicial", value: EnumExigibilidadeISS.SuspensaDecisaoJudicial },
    { text: "Suspensa por Processo Administrativo", value: EnumExigibilidadeISS.SuspensaProcessoAdministrativo }
]

var ImpostoISS = function (nfe) {

    var instancia = this;

    this.BaseISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ISS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.BaseDeducaoISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC Dedução ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.OutrasRetencoesISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Outras Retenções ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.DescontoIncondicionalISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desc. Incondicional ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.DescontoCondicionalISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desc. Condicional ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.RetencaoISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Retenção ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ExigibilidadeISS = PropertyEntity({ val: ko.observable(0), def: 0, options: _exigibilidadeISS, text: "Exigibilidade ISS:", required: false, enable: ko.observable(true) });
    this.IncentivoFiscal = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gera Incentivo Fiscal", def: false, enable: ko.observable(true) });
    this.ProcessoJudicialISS = PropertyEntity({ text: "Processo Judicial: ", required: false, maxlength: 30, val: ko.observable(""), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoISS);
    }

    this.DestivarImpostoISS = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    }

    this.HabilitarImpostoISS = function () {
        HabilitarCamposInstanciasNFe(instancia);
    }

    this.CalcularImpostoISSInstancia = function () {
        CalcularImpostoISS(instancia);
    }
}

function CalcularImpostoISS(instancia) {
    var base = Globalize.parseFloat(instancia.BaseISS.val());
    var aliquota = Globalize.parseFloat(instancia.AliquotaISS.val());

    if (base > 0 && aliquota > 0) {
        var valorISS = base * (aliquota / 100);
        instancia.ValorISS.val(Globalize.format(valorISS, "n2"));
    } else
        instancia.ValorISS.val(Globalize.format(0, "n2"));
}