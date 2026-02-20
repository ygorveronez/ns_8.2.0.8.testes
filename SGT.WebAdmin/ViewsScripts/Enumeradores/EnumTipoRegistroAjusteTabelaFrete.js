var EnumTipoRegistroAjusteTabelaFreteHelper = function () {
    this.Todos = "";
    this.Alterados = 0;
    this.SemAlteracao = 1;
};

EnumTipoRegistroAjusteTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Apenas os alterados", value: this.Alterados },
            { text: "Apenas os sem alteração", value: this.SemAlteracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoRegistroAjusteTabelaFrete = Object.freeze(new EnumTipoRegistroAjusteTabelaFreteHelper());
