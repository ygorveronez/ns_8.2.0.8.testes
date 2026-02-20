var _configuracaoPamcard;

var ConfiguracaoPamcard = function () {
    this.TipoVeiculoPamcard = PropertyEntity({ text: "Tipo de Veículo:", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 2 });
};

function LoadConfiguracaoPamcard() {
    _configuracaoPamcard = new ConfiguracaoPamcard();
    KoBindings(_configuracaoPamcard, "tabPamcard");

    _modeloVeicularCarga.TipoVeiculoPamcard = _configuracaoPamcard.TipoVeiculoPamcard;
}