var _configuracaoA52;

var ConfiguracaoA52 = function () {
    this.TipoVeiculoA52 = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.TipoDeVeiculo.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 2 });
};

function LoadConfiguracaoA52() {
    _configuracaoA52 = new ConfiguracaoA52();
    KoBindings(_configuracaoA52, "tabA52");

    _modeloVeicularCarga.TipoVeiculoA52 = _configuracaoA52.TipoVeiculoA52;
}