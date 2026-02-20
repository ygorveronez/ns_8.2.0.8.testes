var EnumSituacaoPessoaHelper = function () {
    this.Todos = "";
    this.Ativo = true;
    this.Inativo = false;
};

EnumSituacaoPessoaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoPessoa.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.SituacaoPessoa.Ativo, value: this.Ativo },
            { text: Localization.Resources.Enumeradores.SituacaoPessoa.Inativo, value: this.Inativo },
        ];
    },
}

var EnumSituacaoPessoa = Object.freeze(new EnumSituacaoPessoaHelper());