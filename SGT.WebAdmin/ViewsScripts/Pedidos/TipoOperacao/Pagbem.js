var _configuracaoPagbem;

var ConfiguracaoPagbem = function () {
    this.PossuiIntegracaoANTT = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComConsultaDaANTT, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.FreteTipoPesoPagBem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FreteTipoPeso.getFieldDescription(), maxlength: 100, getType: typesKnockout.string });

    this.DataEntregaPagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DataDeEntrega, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.PesoPagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Peso, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.TicketBalancaPagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TicketBalanca, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.AvariaPagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Avaria, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.CanhotoNFePagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CanhotoDaNFe, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.ComprovantePedagioPagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ComprovanteDePedagio, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.DACTEPagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DACTE, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.ContratoTransportePagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ContratoDeTransporte, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.DataDesembarquePagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DataDeDesembarque, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.RelatorioInspecaoDesembarquePagbem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RelatorioDeInspecaoDeDesembarque, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
};

function LoadConfiguracaoPagbem() {
    _configuracaoPagbem = new ConfiguracaoPagbem();
    KoBindings(_configuracaoPagbem, "tabPagbem");

    _tipoOperacao.PossuiIntegracaoANTT = _configuracaoPagbem.PossuiIntegracaoANTT;
    _tipoOperacao.DataEntregaPagbem = _configuracaoPagbem.DataEntregaPagbem;
    _tipoOperacao.PesoPagbem = _configuracaoPagbem.PesoPagbem;
    _tipoOperacao.TicketBalancaPagbem = _configuracaoPagbem.TicketBalancaPagbem;
    _tipoOperacao.AvariaPagbem = _configuracaoPagbem.AvariaPagbem;
    _tipoOperacao.CanhotoNFePagbem = _configuracaoPagbem.CanhotoNFePagbem;
    _tipoOperacao.ComprovantePedagioPagbem = _configuracaoPagbem.ComprovantePedagioPagbem;
    _tipoOperacao.DACTEPagbem = _configuracaoPagbem.DACTEPagbem;
    _tipoOperacao.ContratoTransportePagbem = _configuracaoPagbem.ContratoTransportePagbem;
    _tipoOperacao.DataDesembarquePagbem = _configuracaoPagbem.DataDesembarquePagbem;
    _tipoOperacao.RelatorioInspecaoDesembarquePagbem = _configuracaoPagbem.RelatorioInspecaoDesembarquePagbem;
    _tipoOperacao.FreteTipoPesoPagBem = _configuracaoPagbem.FreteTipoPesoPagBem;
}