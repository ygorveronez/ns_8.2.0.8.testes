/// <reference path="TipoTerceiro.js" />
/// <reference path="../../Enumeradores/EnumTipoTerceiroConfiguracaoContratoFrete.js" />

var _configuracaoContratoFrete;

//*******MAPEAMENTO KNOUCKOUT*******

var ConfiguracaoContratoFrete = function () {
    this.AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.TipoTerceiro.AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado.getFieldDescription(), def: false, visible: true });
    this.EspecificarConfiguracaoContratoFreteTipoTerceiro = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.TipoTerceiro.EspecificarConfiguracaoContratoFreteTipoTerceiro.getFieldDescription(), def: false, visible: true });
    this.ConfiguracaoPercentualAdiantamentoContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa), options: EnumTipoTerceiroConfiguracaoContratoFrete.obterOpcoes(), def: EnumFormaGeracaoTituloFatura.PorPessoa, text: Localization.Resources.Pessoas.TipoTerceiro.ConfiguracaoPercentualAdiantamentoContratoFrete.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfiguracaoPercentualAbastecimentoContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa), options: EnumTipoTerceiroConfiguracaoContratoFrete.obterOpcoes(), def: EnumFormaGeracaoTituloFatura.PorPessoa, text: Localization.Resources.Pessoas.TipoTerceiro.ConfiguracaoPercentualAbastecimentoContratoFrete.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa), options: EnumTipoTerceiroConfiguracaoContratoFrete.obterOpcoes(), def: EnumFormaGeracaoTituloFatura.PorPessoa, text: Localization.Resources.Pessoas.TipoTerceiro.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ConfiguracaoDiasVencimentoSaldoContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa), options: EnumTipoTerceiroConfiguracaoContratoFrete.obterOpcoes(), def: EnumFormaGeracaoTituloFatura.PorPessoa, text: Localization.Resources.Pessoas.TipoTerceiro.ConfiguracaoDiasVencimentoSaldoContratoFrete.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualAdiantamentoFretesTerceiro = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.PercentualAdiantamentoFretesTerceiro.getFieldDescription(), visible: ko.observable(true), required: ko.observable(false), getType: typesKnockout.decimal, maxlength: 6, configDecimal: { precision: 2, allowZero: true }, val: ko.observable("0,00"), enable: ko.observable(false) });
    this.PercentualAbastecimentoFretesTerceiro = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.PercentualAbastecimentoFretesTerceiro.getFieldDescription(), visible: ko.observable(true), required: false, getType: typesKnockout.decimal, maxlength: 6, val: ko.observable("0,00"), enable: ko.observable(false) });
    this.DiasVencimentoAdiantamentoContratoFrete = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.DiasVencimentoAdiantamentoContratoFrete.getFieldDescription(), getType: typesKnockout.int, maxlength: 3, visible: ko.observable(true), val: ko.observable("0"), enable: ko.observable(false) });
    this.DiasVencimentoSaldoContratoFrete = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.DiasVencimentoSaldoContratoFrete.getFieldDescription(), getType: typesKnockout.int, maxlength: 3, visible: ko.observable(true), val: ko.observable("0"), enable: ko.observable(false) });

    this.ConfiguracaoPercentualAdiantamentoContratoFrete.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro) {
            _configuracaoContratoFrete.PercentualAdiantamentoFretesTerceiro.enable(true);
        }
        else {
            _configuracaoContratoFrete.PercentualAdiantamentoFretesTerceiro.val("0,00");
            _configuracaoContratoFrete.PercentualAdiantamentoFretesTerceiro.enable(false);
        }
    });

    this.ConfiguracaoPercentualAbastecimentoContratoFrete.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro) {
            _configuracaoContratoFrete.PercentualAbastecimentoFretesTerceiro.enable(true);
        }
        else {
            _configuracaoContratoFrete.PercentualAbastecimentoFretesTerceiro.val("0,00");
            _configuracaoContratoFrete.PercentualAbastecimentoFretesTerceiro.enable(false);
        }
    });

    this.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro) {
            _configuracaoContratoFrete.DiasVencimentoAdiantamentoContratoFrete.enable(true);
        }
        else {
            _configuracaoContratoFrete.DiasVencimentoAdiantamentoContratoFrete.val(0);
            _configuracaoContratoFrete.DiasVencimentoAdiantamentoContratoFrete.enable(false);
        }
    });

    this.ConfiguracaoDiasVencimentoSaldoContratoFrete.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro) {
            _configuracaoContratoFrete.DiasVencimentoSaldoContratoFrete.enable(true);
        }
        else {
            _configuracaoContratoFrete.DiasVencimentoSaldoContratoFrete.val(0);
            _configuracaoContratoFrete.DiasVencimentoSaldoContratoFrete.enable(false);
        }
    });
}

//*******EVENTOS*******

function loadConfiguracaoContratoFrete() {
    _configuracaoContratoFrete = new ConfiguracaoContratoFrete();
    KoBindings(_configuracaoContratoFrete, "knockoutConfiguracaoContratoFrete");
}

//*******MÉTODOS*******