var _configuracaoAX;

var ConfiguracaoAX = function () {
    this.NaoRealizarIntegracaoComAX = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoRealizarIntegracaoComAX, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });

};

function LoadConfiguracaoAX() {
    _configuracaoAX = new ConfiguracaoAX();
    KoBindings(_configuracaoAX, "tabAX");

    _tipoOperacao.NaoRealizarIntegracaoComAX = _configuracaoAX.NaoRealizarIntegracaoComAX;
}