var EnumTipoFilialContratantePagbemHelper = function () {
    this.Empresa = 0;
    this.GrupoPessoas = 1;
}

EnumTipoFilialContratantePagbemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Empresa", value: this.Empresa },
            { text: "Grupo de Pessoas", value: this.GrupoPessoas }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoFilialContratantePagbem = Object.freeze(new EnumTipoFilialContratantePagbemHelper());