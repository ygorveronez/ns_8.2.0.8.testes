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

var ImpostoICMSMonoRet = function (nfe) {

    var instancia = this;
    
    this.AliquotaRemICMSRet = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Alíquota ICMS Ret:", getType: typesKnockout.decimal, maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSMonoRet = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Mono Ret:", getType: typesKnockout.decimal, maxlength: 18, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutImpostoICMSMonoRet );
    };

    this.DestivarImpostoICMSMonoRet = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarImpostoICMSMonoRet = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };

    this.CalcularImpostoICMSMonoRetInstancia = function () {
        var baseICMS = 0;        
        var aliquotaICMS = Globalize.parseFloat(instancia.AliquotaRemICMSRet.val());

        var valorTotalItem = Globalize.parseFloat(nfe.ProdutoServico.ValorTotalItem.val());
        if (valorTotalItem > 0)
            baseICMS = valorTotalItem;

        if (baseICMS > 0 && aliquotaICMS > 0) {            
            var valorICMS = baseICMS * (aliquotaICMS / 100);
            
            instancia.ValorICMSMonoRet.val(Globalize.format(valorICMS, "n2"));
        } else
            instancia.ValorICMSMonoRet.val(Globalize.format(0, "n2"));
    };
};

function CalcularImpostoICMSMonoRet(instancia) {
    instancia.CalcularImpostoICMSMonoRetInstancia();
}