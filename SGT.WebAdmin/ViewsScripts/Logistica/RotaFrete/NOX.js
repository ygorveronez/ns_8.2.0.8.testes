var _configuracaoNOX;

var ConfiguracaoNOX = function () {
    this.CodigoIntegracaoNOX = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CodigoIntegracao, val: ko.observable(""), def: "" });
};

function LoadConfiguracaoNOX() {
    _configuracaoNOX = new ConfiguracaoNOX();
    KoBindings(_configuracaoNOX, "knockoutNOX");

    _rotaFrete.CodigoIntegracaoNOX = _configuracaoNOX.CodigoIntegracaoNOX;
}