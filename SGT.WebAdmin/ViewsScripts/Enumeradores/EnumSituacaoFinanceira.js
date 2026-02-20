var EnumSituacaoFinanceiraHelper = function () {
    this.Todos = "";
    this.Liberada = 0;
    this.Bloqueada = 1;
};

EnumSituacaoFinanceiraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoFinanceiraPessoas.Liberada, value: this.Liberada },
            { text: Localization.Resources.Enumeradores.SituacaoFinanceiraPessoas.Bloqueada, value: this.Bloqueada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoFinanceiraPessoas.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoFinanceira = Object.freeze(new EnumSituacaoFinanceiraHelper());