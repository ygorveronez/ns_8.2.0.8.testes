var EnumSituacaoAutorizacaoHelper = function () {
    this.Todos = 0;
    this.PendenteAutorizacao = 1;
    this.SomenteAutorizados = 2;
};

EnumSituacaoAutorizacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendentes de Autorização", value: this.PendenteAutorizacao },
            { text: "Somente Autorizados", value: this.SomenteAutorizados }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAutorizacao = Object.freeze(new EnumSituacaoAutorizacaoHelper());