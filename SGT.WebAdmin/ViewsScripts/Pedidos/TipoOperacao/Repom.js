var _configuracaoRepom;

var ConfiguracaoRepom = function () {
    this.CodigoIntegracaoRepom = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CodigoIntegracao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 2 });
};

function LoadConfiguracaoRepom() {
    _configuracaoRepom = new ConfiguracaoRepom();
    KoBindings(_configuracaoRepom, "tabRepom");

    _tipoOperacao.CodigoIntegracaoRepom = _configuracaoRepom.CodigoIntegracaoRepom;
}