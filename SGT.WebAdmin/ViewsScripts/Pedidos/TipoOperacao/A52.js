var _configuracaoA52;

var ConfiguracaoA52 = function () {
    this.PossuiIntegracaoA52 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComA52, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });

    this.TempoEntregaA52 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoParaEntregaMinutos.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3 });
    this.TipoA52 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Tipo.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3 });
    this.TipoCargaA52 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3 });
    this.TipoOperacaoA52 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOperacao.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 200 });
    this.IntegrarPedidosA52 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarPedidosA52, val: ko.observable(false), def: false, getType: typesKnockout.bool });
};

function LoadConfiguracaoA52() {
    _configuracaoA52 = new ConfiguracaoA52();
    KoBindings(_configuracaoA52, "tabA52");

    _tipoOperacao.PossuiIntegracaoA52 = _configuracaoA52.PossuiIntegracaoA52;
    _tipoOperacao.TempoEntregaA52 = _configuracaoA52.TempoEntregaA52;
    _tipoOperacao.TipoA52 = _configuracaoA52.TipoA52;
    _tipoOperacao.TipoCargaA52 = _configuracaoA52.TipoCargaA52;
    _tipoOperacao.TipoOperacaoA52 = _configuracaoA52.TipoOperacaoA52;
    _tipoOperacao.IntegrarPedidosA52 = _configuracaoA52.IntegrarPedidosA52;

}