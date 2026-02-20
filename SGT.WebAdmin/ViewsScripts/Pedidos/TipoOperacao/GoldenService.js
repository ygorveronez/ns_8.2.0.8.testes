var _configuracaoGoldenService;


var ConfiguracaoGoldenService = function () {
    this.PossuiIntegracaoGoldenService = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComGoldenService, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.CodigoIntegracaoGoldenService = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CodigoDeIntegracao.getFieldDescription(), val: ko.observable(""), def: "" });
};

function LoadConfiguracaoGoldenService() {
    _configuracaoGoldenService = new ConfiguracaoGoldenService();
    KoBindings(_configuracaoGoldenService, "tabGoldenService");

    _tipoOperacao.CodigoIntegracaoGoldenService = _configuracaoGoldenService.CodigoIntegracaoGoldenService;
    _tipoOperacao.PossuiIntegracaoGoldenService = _configuracaoGoldenService.PossuiIntegracaoGoldenService;
}