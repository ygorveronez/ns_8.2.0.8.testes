var _configuracaoTarget;

var ConfiguracaoTarget = function () {
    this.CategoriaVeiculoTarget = PropertyEntity({ text: "Categoria do Veículo:", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 2 });
};

function LoadConfiguracaoTarget() {
    _configuracaoTarget = new ConfiguracaoTarget();
    KoBindings(_configuracaoTarget, "tabTarget");

    _modeloVeicularCarga.CategoriaVeiculoTarget = _configuracaoTarget.CategoriaVeiculoTarget;
}