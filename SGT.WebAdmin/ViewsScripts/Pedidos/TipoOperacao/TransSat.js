var _configuracaoTransSat;

var ConfiguracaoTransSat = function () {
    this.PossuiIntegracaoTransSat = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComTransSat, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
};

function LoadConfiguracaoTransSat() {
    _configuracaoTransSat = new ConfiguracaoTransSat();
    KoBindings(_configuracaoTransSat, "tabTransSat");

    _tipoOperacao.PossuiIntegracaoTransSat = _configuracaoTransSat.PossuiIntegracaoTransSat;
}