var EnumTipoCondicaoPagamentoHelper = function () {
    this.Todos = null;
    this.CIF = 1;
    this.FOB = 2;
};

EnumTipoCondicaoPagamentoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.CIF, value: this.CIF },
            { text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.FOB, value: this.FOB }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.Todos, value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumTipoCondicaoPagamento = Object.freeze(new EnumTipoCondicaoPagamentoHelper());
