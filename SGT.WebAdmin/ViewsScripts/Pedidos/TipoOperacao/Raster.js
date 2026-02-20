var _configuracaoRaster;

var ConfiguracaoRaster = function () {
    this.CodigoFilial = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CodigoDaFilial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 10 });
    this.CodigoPerfilSeguranca = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CodigoDoPerfilDeSeguranca.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 10 });
    this.PossuiIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComRaster, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
};

function LoadConfiguracaoRaster() {
    _configuracaoRaster = new ConfiguracaoRaster();
    KoBindings(_configuracaoRaster, "tabRaster");

    _tipoOperacao.CodigoFilialRaster = _configuracaoRaster.CodigoFilial;
    _tipoOperacao.CodigoPerfilSegurancaRaster = _configuracaoRaster.CodigoPerfilSeguranca;
    _tipoOperacao.PossuiIntegracaoRaster = _configuracaoRaster.PossuiIntegracao;
}