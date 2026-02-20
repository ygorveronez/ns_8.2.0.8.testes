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
/// <reference path="NFSe.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _SimNao = [
    { text: "Sim", value: EnumSimNao.Sim },
    { text: "Não", value: EnumSimNao.Nao }
];

var Valor = function (nfse) {

    var instancia = this;

    this.ValorTotalServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total Serviços:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalLiquido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total Líquido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseDeducao = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Dedução:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaIBSEstadual = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota IBS Estadual:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota IBS Municipal:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIBSEstadual = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IBS Estadual:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IBS Municipal:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota CBS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor CBS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorINSS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor INSS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIR = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IR:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.ValorCSLL = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor CSLL:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.RetencaoISS = PropertyEntity({ val: ko.observable(EnumSimNao.Nao), def: EnumSimNao.Nao, options: _SimNao, text: "ISS Retido?", required: true, enable: ko.observable(true) });
    this.ValorRetencaoISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ISS Retido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorOutrasRetencoes = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Outras Retenções:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorDescontoIncondicional = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desconto Incondicional:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorDescontoCondicional = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desconto Condicional:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.BaseISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaISS = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Alíquota ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 4, allowZero: true } });
    this.ValorISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.BasePIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaPIS = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Alíquota PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 4, allowZero: true } });
    this.AliquotaCOFINS = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Alíquota COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 4, allowZero: true } });
    this.ValorPIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfse.IdKnockoutValor);
    };

    this.DestivarTotalizador = function () {
        DesabilitarCamposInstanciasNFSe(instancia);
    };

    this.ValorRetencaoISS.val.subscribe(function () {
        AtualizarValorTotalLiquido(instancia);
    });

    this.RetencaoISS.val.subscribe(function () {
        CalcularValorRetencaoISS(instancia, nfse);
    });
};

function AtualizarValorTotalLiquido(instancia) {
    var valorTotalServicos = Globalize.parseFloat(instancia.ValorTotalServicos.val());
    var valorISSRetido = Globalize.parseFloat(instancia.ValorRetencaoISS.val());
    var valorliquidoTotal = valorTotalServicos - valorISSRetido;
    if (valorliquidoTotal > 0)
        instancia.ValorTotalLiquido.val(Globalize.format(valorliquidoTotal, "n2"));
    else
        instancia.ValorTotalLiquido.val(Globalize.format(valorTotalServicos, "n2"));
}

function CalcularValorRetencaoISS(instancia, nfse) {
    instancia.ValorRetencaoISS.enable(false);
    var retencaoISS = instancia.RetencaoISS.val();

    if (retencaoISS == EnumSimNao.Sim && nfse.Servicos.length > 0) {
        instancia.ValorRetencaoISS.enable(true);
        var valorRetencao = 0;
        for (var i = 0; i < nfse.Servicos.length; i++) {
            var servico = nfse.Servicos[i];
            var baseISS = Globalize.parseFloat(servico.BCISS);
            var aliquotaISS = Globalize.parseFloat(servico.AliquotaISS);
            valorRetencao += baseISS * (aliquotaISS / 100);

            nfse.Servicos[i].ValorISS = Globalize.format(0, "n2");//Quando tem ISS retido, zera o ISS normal
        }

        instancia.ValorRetencaoISS.val(Globalize.format(valorRetencao, "n2"));
        instancia.ValorISS.val(Globalize.format(0, "n2"));
    }
    else {
        var valorISSTotal = 0;
        for (var i = 0; i < nfse.Servicos.length; i++) {
            var servico = nfse.Servicos[i];
            var baseISS = Globalize.parseFloat(servico.BCISS);
            var aliquotaISS = Globalize.parseFloat(servico.AliquotaISS);
            var valorISS = baseISS * (aliquotaISS / 100);
            valorISSTotal += valorISS;

            nfse.Servicos[i].ValorISS = Globalize.format(valorISS, "n2");
        }

        instancia.ValorISS.val(Globalize.format(valorISSTotal, "n2"));
        instancia.ValorRetencaoISS.val(Globalize.format(0, "n2"));
    }

    nfse.ListaServico.RecarregarGrid();
    AtualizarValorTotalLiquido(instancia);
}