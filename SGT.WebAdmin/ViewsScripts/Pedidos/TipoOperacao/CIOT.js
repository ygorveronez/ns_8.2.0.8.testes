var _configuracaoCIOT;

var ConfiguracaoCIOT = function () {
    this.ConfiguracaoCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.Operadora.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.UtilizarConfiguracaoPersonalizadaParcelasPamcard = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarConfiguracaoPersonalizadaParaAsParcelasDoCIOT, val: ko.observable(false), def: false, visible: ko.observable(false), getType: typesKnockout.bool });

    this.EfetivacaoAdiantamentoPamcard = PropertyEntity({ val: ko.observable(""), options: EnumPamcardParcelaTipoEfetivacao.obterOpcoes({ value: "", text: Localization.Resources.Pedidos.TipoOperacao.NaoSelecionado }), def: "", text: Localization.Resources.Pedidos.TipoOperacao.EfetivacaoDoAdiantamento.getFieldDescription(), issue: 0, visible: ko.observable(true) });
    this.StatusAdiantamentoPamcard = PropertyEntity({ val: ko.observable(""), options: EnumPamcardParcelaStatus.obterOpcoes({ value: "", text: Localization.Resources.Pedidos.TipoOperacao.NaoSelecionado }), def: "", text: Localization.Resources.Pedidos.TipoOperacao.StatusDoAdiantamento.getFieldDescription(), issue: 0, visible: ko.observable(true) });

    this.EfetivacaoAbastecimentoPamcard = PropertyEntity({ val: ko.observable(""), options: EnumPamcardParcelaTipoEfetivacao.obterOpcoes({ value: "", text: Localization.Resources.Pedidos.TipoOperacao.NaoSelecionado }), def: "", text: Localization.Resources.Pedidos.TipoOperacao.EfetivacaoDoAbastecimento.getFieldDescription(), issue: 0, visible: ko.observable(true) });
    this.StatusAbastecimentoPamcard = PropertyEntity({ val: ko.observable(""), options: EnumPamcardParcelaStatus.obterOpcoes({ value: "", text: Localization.Resources.Pedidos.TipoOperacao.NaoSelecionado }), def: "", text: Localization.Resources.Pedidos.TipoOperacao.StatusDoAbastecimento.getFieldDescription(), issue: 0, visible: ko.observable(true) });

    this.EfetivacaoSaldoPamcard = PropertyEntity({ val: ko.observable(""), options: EnumPamcardParcelaTipoEfetivacao.obterOpcoes({ value: "", text: Localization.Resources.Pedidos.TipoOperacao.NaoSelecionado }), def: "", text: Localization.Resources.Pedidos.TipoOperacao.EfetivacaoDoSaldo.getFieldDescription(), issue: 0, visible: ko.observable(true) });
    this.StatusSaldoPamcard = PropertyEntity({ val: ko.observable(""), options: EnumPamcardParcelaStatus.obterOpcoes({ value: "", text: Localization.Resources.Pedidos.TipoOperacao.NaoSelecionado }), def: "", text: Localization.Resources.Pedidos.TipoOperacao.StatusDoSaldo.getFieldDescription(), issue: 0, visible: ko.observable(true) });

    this.TipoOperadoraCIOT = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.OperadoraDoCIOT, val: ko.observable(0), def: 0, visible: ko.observable(false), getType: typesKnockout.int });
};

function LoadConfiguracaoCIOT() {
    _configuracaoCIOT = new ConfiguracaoCIOT();
    KoBindings(_configuracaoCIOT, "tabCIOT");

    BuscarConfiguracaoCIOT(_configuracaoCIOT.ConfiguracaoCIOT, Localization.Resources.Pedidos.TipoOperacao.ConsultaDeOperadorasDeCIOT, Localization.Resources.Pedidos.TipoOperacao.OperadorasDeCIOT, RetornoConsultaConfiguracaoCIOT);

    _tipoOperacao.ConfiguracaoCIOT = _configuracaoCIOT.ConfiguracaoCIOT;

    _tipoOperacao.UtilizarConfiguracaoPersonalizadaParcelasPamcard = _configuracaoCIOT.UtilizarConfiguracaoPersonalizadaParcelasPamcard;

    _tipoOperacao.EfetivacaoAdiantamentoPamcard = _configuracaoCIOT.EfetivacaoAdiantamentoPamcard;
    _tipoOperacao.StatusAdiantamentoPamcard = _configuracaoCIOT.StatusAdiantamentoPamcard;

    _tipoOperacao.EfetivacaoAbastecimentoPamcard = _configuracaoCIOT.EfetivacaoAbastecimentoPamcard;
    _tipoOperacao.StatusAbastecimentoPamcard = _configuracaoCIOT.StatusAbastecimentoPamcard;

    _tipoOperacao.EfetivacaoSaldoPamcard = _configuracaoCIOT.EfetivacaoSaldoPamcard;
    _tipoOperacao.StatusSaldoPamcard = _configuracaoCIOT.StatusSaldoPamcard;

    _tipoOperacao.TipoOperadoraCIOT = _configuracaoCIOT.TipoOperadoraCIOT;
}

function RetornoConsultaConfiguracaoCIOT(data) {
    _configuracaoCIOT.ConfiguracaoCIOT.val(data.Descricao);
    _configuracaoCIOT.ConfiguracaoCIOT.codEntity(data.Codigo);

    _configuracaoCIOT.TipoOperadoraCIOT.val(data.TipoOperadoraCIOT);

    _configuracaoCIOT.UtilizarConfiguracaoPersonalizadaParcelasPamcard.visible(data.TipoOperadoraCIOT === EnumOperadoraCIOT.Pamcard);
}
