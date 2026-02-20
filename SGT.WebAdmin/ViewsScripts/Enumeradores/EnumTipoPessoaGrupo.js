var EnumTipoPessoaGrupoHelper = function () {
    this.Todos = "";
    this.Pessoa = 1;
    this.GrupoPessoa = 2;
};

EnumTipoPessoaGrupoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPessoaGrupo.Pessoa, value: this.Pessoa },
            { text: Localization.Resources.Enumeradores.TipoPessoaGrupo.GrupoPessoa, value: this.GrupoPessoa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPessoaGrupo.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPessoaGrupo = Object.freeze(new EnumTipoPessoaGrupoHelper());