var _configuracaoBBC = null;

var ConfiguracaoBBC = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function LoadConfiguracaoBBC() {
    _configuracaoBBC = new ConfiguracaoBBC();
    KoBindings(_configuracaoBBC, "tabBBC");
}

function LimparCamposConfiguracaoBBC() {
    LimparCampos(_configuracaoBBC);
}