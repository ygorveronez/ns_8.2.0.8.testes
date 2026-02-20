var _configuracaoTerceiro;


var ConfiguracaoTerceiro = function () {
    this.UtilizarConfiguracaoTerceiro = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EspecificarOutraConfiguracaoParaFretesDeTerceirosParaEsseTipoDeOperacao, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.UtilizarConfiguracaoTerceiroComoPadrao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DeveSerUtilizadaComoPadraoConfiguracaoDoTerceiroIraSobreporConfiguracaoDaOperacao, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PercentualAdiantamentoFreteTerceiro = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.PorcentagemDeAdiantamento.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PercentualAbastecimentoFreteTerceiro = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.PorcentagemDeAbastecimento.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PercentualCobrancaPadraoTerceiros = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.PorcentagemDeDescontoQuandoSubcontrataTerceiros.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.DiasVencimentoAdiantamentoContratoFrete = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Pedidos.TipoOperacao.DiasVencimentoAdiantamento.getFieldDescription(), val: ko.observable(""), def: "", configInt: { precision: 0, allowZero: false, allowNegative: false } });
    this.DiasVencimentoSaldoContratoFrete = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Pedidos.TipoOperacao.DiasVencimentoSaldo.getFieldDescription(), val: ko.observable(""), def: "", configInt: { precision: 0, allowZero: false, allowNegative: false } });
    this.NaoGerarContratoFreteTerceiro = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarContratoDeFreteOuCIOTParaTerceiros, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoPagamentoContratoFreteTerceiro = PropertyEntity({ val: ko.observable(""), options: EnumTipoPagamentoContratoFreteTerceiro.obterOpcoes("", Localization.Resources.Pedidos.TipoOperacao.UtilizarDadrao), def: "", text: Localization.Resources.Pedidos.TipoOperacao.TipoDeCalculo.getFieldDescription() });
    this.CodigoCondicaoPagamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CodigoCondicaoDePagamento.getFieldDescription(), maxlength: 50 });
    this.NaoSomarValorPedagioContratoFrete = PropertyEntity({ text: "Não somar o valor do pedágio no contrato de frete", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.NaoSubtrairValePedagioDoContrato = PropertyEntity({ text: "Não subtrair o vale pedágio do contrato de frete", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato.val.subscribe(function (novoValor) {
        _tipoOperacao.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato.visible(novoValor);
        _tipoOperacao.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato.required(novoValor);
    });
};

function LoadConfiguracaoTerceiro() {
    _configuracaoTerceiro = new ConfiguracaoTerceiro();
    KoBindings(_configuracaoTerceiro, "tabTerceiro");

    new BuscarJustificativas(_configuracaoTerceiro.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato);

    _tipoOperacao.UtilizarConfiguracaoTerceiro = _configuracaoTerceiro.UtilizarConfiguracaoTerceiro;
    _tipoOperacao.PercentualAdiantamentoFreteTerceiro = _configuracaoTerceiro.PercentualAdiantamentoFreteTerceiro;
    _tipoOperacao.PercentualAbastecimentoFreteTerceiro = _configuracaoTerceiro.PercentualAbastecimentoFreteTerceiro;
    _tipoOperacao.UtilizarConfiguracaoTerceiroComoPadrao = _configuracaoTerceiro.UtilizarConfiguracaoTerceiroComoPadrao;
    _tipoOperacao.PercentualCobrancaPadraoTerceiros = _configuracaoTerceiro.PercentualCobrancaPadraoTerceiros;
    _tipoOperacao.CodigoCondicaoPagamento = _configuracaoTerceiro.CodigoCondicaoPagamento;
    _tipoOperacao.DiasVencimentoAdiantamentoContratoFrete = _configuracaoTerceiro.DiasVencimentoAdiantamentoContratoFrete;
    _tipoOperacao.DiasVencimentoSaldoContratoFrete = _configuracaoTerceiro.DiasVencimentoSaldoContratoFrete;
    _tipoOperacao.NaoGerarContratoFreteTerceiro = _configuracaoTerceiro.NaoGerarContratoFreteTerceiro;
    _tipoOperacao.TipoPagamentoContratoFreteTerceiro = _configuracaoTerceiro.TipoPagamentoContratoFreteTerceiro;
    _tipoOperacao.NaoSomarValorPedagioContratoFrete = _configuracaoTerceiro.NaoSomarValorPedagioContratoFrete;
    _tipoOperacao.NaoSubtrairValePedagioDoContrato = _configuracaoTerceiro.NaoSubtrairValePedagioDoContrato;
    _tipoOperacao.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = _configuracaoTerceiro.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato;
    _tipoOperacao.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = _configuracaoTerceiro.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato;
}