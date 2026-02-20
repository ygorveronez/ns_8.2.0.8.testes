var _configuracaoMundialRisk;

var ConfiguracaoMundialRisk = function () {
    this.CentroCustos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CentroDeCustos.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CNPJTransportadora = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CNPJDaTransportadora.getFieldDescription(), val: ko.observable(""), def: "", required: false });
    this.CNPJCliente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CNPJDoCliente.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Produto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Produto.getFieldDescription(), val: ko.observable(""), def: "" });

    this.PossuiIntegracaoMundialRisk = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarintegracaoDeInicioDeViagemComMundialRisk, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.PossuiIntegracaoMundialRisk.val.subscribe(function (novoValor) {
        if (novoValor)
            _configuracaoMundialRisk.CNPJTransportadora.required = true;
        else
            _configuracaoMundialRisk.CNPJTransportadora.required = false;
    });
}

function LoadConfiguracaoMundialRisk() {
    _configuracaoMundialRisk = new ConfiguracaoMundialRisk();
    KoBindings(_configuracaoMundialRisk, "tabMundialRisk");

    _tipoOperacao.PossuiIntegracaoMundialRisk = _configuracaoMundialRisk.PossuiIntegracaoMundialRisk;
    _tipoOperacao.CentroCustoMundialRisk = _configuracaoMundialRisk.CentroCustos;
    _tipoOperacao.CNPJTransportadoraMundialRisk = _configuracaoMundialRisk.CNPJTransportadora;
    _tipoOperacao.CNPJClienteMundialRisk = _configuracaoMundialRisk.CNPJCliente;
    _tipoOperacao.ProdutoMundialRisk = _configuracaoMundialRisk.Produto;
}