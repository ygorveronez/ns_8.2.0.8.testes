var _configuracaoGoldenService;

var ConfiguracaoGoldenService = function () {
    this.CodigoIntegracaoGoldenService = PropertyEntity({ text: "Código de Integração:", val: ko.observable(""), def: "", visible: ko.observable(true) });
};

function LoadConfiguracaoGoldenService() {
    _configuracaoGoldenService = new ConfiguracaoGoldenService();
    KoBindings(_configuracaoGoldenService, "tabGoldenService");

    _modeloVeicularCarga.CodigoIntegracaoGoldenService = _configuracaoGoldenService.CodigoIntegracaoGoldenService;
}