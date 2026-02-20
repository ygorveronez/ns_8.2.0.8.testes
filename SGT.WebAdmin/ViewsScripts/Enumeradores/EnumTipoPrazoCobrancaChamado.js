var EnumTipoPrazoCobrancaChamadoHelper = function () {
    this.Todos = "";
    this.DataPagamentoDescarga = 1;
    this.DataEmissaoViagem = 2;
};

EnumTipoPrazoCobrancaChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPrazoCobranca.DataPagamentoDescarga, value: this.DataPagamentoDescarga },
            { text: Localization.Resources.Enumeradores.TipoPrazoCobranca.DataEmissaoViagem, value: this.DataEmissaoViagem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPrazoCobranca.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPrazoCobrancaChamado = Object.freeze(new EnumTipoPrazoCobrancaChamadoHelper());