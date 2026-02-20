var _configuracaoRepom;

var ConfiguracaoRepom = function () {
    this.TipoVeiculoRepom = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.TipoDeVeiculo.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 2 });
};

function LoadConfiguracaoRepom() {
    _configuracaoRepom = new ConfiguracaoRepom();
    KoBindings(_configuracaoRepom, "tabRepom");

    _modeloVeicularCarga.TipoVeiculoRepom = _configuracaoRepom.TipoVeiculoRepom;
}