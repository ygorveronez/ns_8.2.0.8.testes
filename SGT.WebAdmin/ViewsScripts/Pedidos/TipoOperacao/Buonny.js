var _configuracaoBuonny;

var ConfiguracaoBuonny = function () {
    this.MonitorarRetornoCargaBuonny = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.MonitorarRetornoDaCarga, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });

};

function LoadConfiguracaoBuonny() {
    _configuracaoBuonny = new ConfiguracaoBuonny();
    KoBindings(_configuracaoBuonny, "tabBuonny");

    _tipoOperacao.MonitorarRetornoCargaBuonny = _configuracaoBuonny.MonitorarRetornoCargaBuonny;
}