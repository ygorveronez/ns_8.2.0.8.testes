var EnumSituacaoAbastecimentoHelper = function () {
    this.Todos = "T";
    this.Aberto = "A";
    this.Fechado = "F";
    this.Inconsistente = "I";
    this.Agrupado = "G";
    this.Requisicao = "R";
};

EnumSituacaoAbastecimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Fechado", value: this.Fechado },
            { text: "Inconsistente", value: this.Inconsistente },
            { text: "Agrupado", value: this.Agrupado }
        ];
    },
    obterOpcoesReprocessarAbastecimento: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Inconsistente", value: this.Inconsistente }
        ];
    },
    obterOpcoesPesquisa: function () {
         return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaRequisicao: function (arg) {
        if(arg)
            return [
                { text: "Todos", value: this.Todos },
                { text: "Aberto", value: this.Aberto },
                { text: "Fechado", value: this.Fechado },
                { text: "Inconsistente", value: this.Inconsistente },
                { text: "Agrupado", value: this.Agrupado },
                { text: "Requisição", value: this.Requisicao }
            ];
        else
            return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAbastecimento = Object.freeze(new EnumSituacaoAbastecimentoHelper());