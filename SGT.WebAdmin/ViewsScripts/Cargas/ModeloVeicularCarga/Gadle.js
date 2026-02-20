var _configuracaoGadle;

var ConfiguracaoGadle = function () {
    this.TipoVeiculoGadle = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.TipoDeVeiculo.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 10 });
};

function LoadConfiguracaoGadle() {
    _configuracaoGadle = new ConfiguracaoGadle();
    KoBindings(_configuracaoGadle, "tabGadle");

    _modeloVeicularCarga.TipoVeiculoGadle = _configuracaoGadle.TipoVeiculoGadle;
}