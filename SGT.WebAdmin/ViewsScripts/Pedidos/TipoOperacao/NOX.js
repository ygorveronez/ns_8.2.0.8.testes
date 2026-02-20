var _configuracaoNOX;

var ConfiguracaoNOX = function () {
    this.PossuiIntegracaoNOX = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComNOX, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.ValorMinimoMercadoriaNOX = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValorMinimoDaMercadoriaParaIntegrar.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, maxlength: 12, enable: ko.observable(true) });
    this.IntegrarPreSMNOX = PropertyEntity({ text: "Integrar pré SM na carga", val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS), getType: typesKnockout.bool });
};

function LoadConfiguracaoNOX() {
    _configuracaoNOX = new ConfiguracaoNOX();
    KoBindings(_configuracaoNOX, "tabNOX");

    $("#" + _configuracaoNOX.IntegrarPreSMNOX.id).click(IntegrarPreSMNOXClick);

    _tipoOperacao.PossuiIntegracaoNOX = _configuracaoNOX.PossuiIntegracaoNOX;
    _tipoOperacao.ValorMinimoMercadoriaNOX = _configuracaoNOX.ValorMinimoMercadoriaNOX;
    _tipoOperacao.IntegrarPreSMNOX = _configuracaoNOX.IntegrarPreSMNOX;
}



function IntegrarPreSMNOXClick() {
    if (_configuracaoNOX.IntegrarPreSMNOX.val()) {
        _configuracaoNOX.ValorMinimoMercadoriaNOX.val("0,00");
    }
}