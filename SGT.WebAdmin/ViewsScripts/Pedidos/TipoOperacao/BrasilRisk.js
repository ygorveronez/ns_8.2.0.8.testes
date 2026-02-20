var _configuracaoBrasilRisk;


var ConfiguracaoBrasilRisk = function () {
    this.PossuiIntegracao = PropertyEntity({ text: "Possui Integração com a BrasilRisk", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PedidoLogistico = PropertyEntity({ text: "Pedido Logístico", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CentroCustos = PropertyEntity({ text: "Centro de Custos:", val: ko.observable(""), def: "" });
    this.CodigoModeloContratacao = PropertyEntity({ text: "Modelo de Contratação:", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 2, visible: ko.observable(true) });
    this.CNPJTransportadora = PropertyEntity({ text: "CNPJ da Transportadora:", val: ko.observable(""), def: "" });
    this.CNPJCliente = PropertyEntity({ text: "CNPJ do Cliente:", val: ko.observable(""), def: "" });
    this.Produto = PropertyEntity({ text: "Produto:", val: ko.observable(""), def: "" });
    this.EnviarNumeroPedidoEmbarcadorNoCodigoControle = PropertyEntity({ text: "Enviar o número do pedido no embarcador no código de controle", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoEnviarOrigemComoUltimoPontoRota = PropertyEntity({ text: "Não enviar origem como último ponto rota", getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

function LoadConfiguracaoBrasilRisk() {
    _configuracaoBrasilRisk = new ConfiguracaoBrasilRisk();
    KoBindings(_configuracaoBrasilRisk, "tabBrasilRisk");

    _tipoOperacao.CodigoModeloContratacao = _configuracaoBrasilRisk.CodigoModeloContratacao;
    _tipoOperacao.PossuiIntegracaoBrasilRisk = _configuracaoBrasilRisk.PossuiIntegracao;
    _tipoOperacao.PedidoLogisticoBrasilRisk = _configuracaoBrasilRisk.PedidoLogistico;
    _tipoOperacao.CentroCustoBrasilRisk = _configuracaoBrasilRisk.CentroCustos;
    _tipoOperacao.CNPJTransportadoraBrasilRisk = _configuracaoBrasilRisk.CNPJTransportadora;
    _tipoOperacao.CNPJClienteBrasilRisk = _configuracaoBrasilRisk.CNPJCliente;
    _tipoOperacao.ProdutoBrasilRisk = _configuracaoBrasilRisk.Produto;
    _tipoOperacao.EnviarNumeroPedidoEmbarcadorNoCodigoControleBrasilRisk = _configuracaoBrasilRisk.EnviarNumeroPedidoEmbarcadorNoCodigoControle;
    _tipoOperacao.NaoEnviarOrigemComoUltimoPontoRota = _configuracaoBrasilRisk.NaoEnviarOrigemComoUltimoPontoRota;
}