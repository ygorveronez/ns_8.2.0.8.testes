var _configuracaoLogiun;

var ConfiguracaoLogiun = function () {
    this.CentroCustos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CentroDeCustos.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CNPJTransportadora = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CNPJDaTransportadora.getFieldDescription(), val: ko.observable(""), def: "", required: false });
    this.CNPJCliente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CNPJDoCliente.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Produto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Produto.getFieldDescription(), val: ko.observable(""), def: "", required: false });

    this.PossuiIntegracaoLogiun = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarIntegracaoDeTransferenciaDePalletsParaLogiu, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.PossuiIntegracaoLogiun.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoLogiun.CNPJTransportadora.required = true;
            _configuracaoLogiun.Produto.required = true;
        }
        else {
            _configuracaoLogiun.CNPJTransportadora.required = false;
            _configuracaoLogiun.Produto.required = false;
        }
    });
}

function LoadConfiguracaoLogiun() {
    _configuracaoLogiun = new ConfiguracaoLogiun();
    KoBindings(_configuracaoLogiun, "tabLogiun");

    _tipoOperacao.PossuiIntegracaoLogiun = _configuracaoLogiun.PossuiIntegracaoLogiun;
    _tipoOperacao.CentroCustoLogiun = _configuracaoLogiun.CentroCustos;
    _tipoOperacao.CNPJTransportadoraLogiun = _configuracaoLogiun.CNPJTransportadora;
    _tipoOperacao.CNPJClienteLogiun = _configuracaoLogiun.CNPJCliente;
    _tipoOperacao.ProdutoLogiun = _configuracaoLogiun.Produto;
}