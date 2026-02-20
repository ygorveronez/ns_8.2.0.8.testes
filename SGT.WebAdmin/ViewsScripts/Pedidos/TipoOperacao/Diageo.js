var _configuracaoDiageo;

var ConfiguracaoDiageo = function () {
    this.PossuiIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComDiageo, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
};

function LoadConfiguracaoDiageo() {
    _configuracaoDiageo = new ConfiguracaoDiageo();
    KoBindings(_configuracaoDiageo, "tabDiageo");

    _tipoOperacao.PossuiIntegracaoDiageo = _configuracaoDiageo.PossuiIntegracao;
}