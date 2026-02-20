var EnumTipoPrazoFaturamentoHelper = function () {
    this.Todos = "";
    this.DataFatura = 1;
    this.DataDocumento = 2;
    this.DataPrevisaoEncerramento = 3;
    this.DataPrevisaoInicioViagem = 4;
    this.ApartirAprovacaoFatura = 5;
    this.ApartirDataGeracaoFatura = 6;
};

EnumTipoPrazoFaturamentoHelper.prototype = {
    obterOpcoes: function () {
        let opcoes = [
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.PartirDaDataFinalDoFaturamento, value: this.DataFatura },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.PartirDaEmissaoDoPrimeiroDocumento, value: this.DataDocumento },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.PartirDaDataDePrevisaoDeEncerramento, value: this.DataPrevisaoEncerramento },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.PartirDaDataDePrevisaoDeInicio, value: this.DataPrevisaoInicioViagem },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.ApartirAprovacaoFatura, value: this.ApartirAprovacaoFatura },
            { text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.ApartirDataGeracaoFatura, value: this.ApartirDataGeracaoFatura }
        ];

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
            opcoes.push({ text: "Nenhum", value: this.Todos });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPrazoFaturamento.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPrazoFaturamento = Object.freeze(new EnumTipoPrazoFaturamentoHelper());