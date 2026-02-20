var EnumSituacaoDoConjuntoFrotaHelper = function () {
    this.Todos = 0;
    this.SemTracao = 1;
    this.SemReboque = 2;
    this.ConjuntoCompleto = 3;
};

EnumSituacaoDoConjuntoFrotaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Tração", value: this.SemTracao },
            { text: "Sem Reboque", value: this.SemReboque },
            { text: "Conjunto Completo", value: this.ConjuntoCompleto }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoDoConjuntoFrota = Object.freeze(new EnumSituacaoDoConjuntoFrotaHelper());