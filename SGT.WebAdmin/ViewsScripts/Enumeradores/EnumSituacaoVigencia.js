var EnumSituacaoVigenciaHelper = function () {
    this.Todos = 0;
    this.ApenasVigentes = 1;
    this.ForaDeVigencia = 2;
};

EnumSituacaoVigenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Apenas Vigentes", value: this.ApenasVigentes },
            { text: "Fora de Vigência", value: this.ApenasNaoVigentes }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoVigencia = Object.freeze(new EnumSituacaoVigenciaHelper());