var EnumSituacaoEstoqueProdutoArmazemHelper = function () {
    this.Todos = 0;
    this.EstoqueTotal = 1;
    this.EstoqueParcial = 2;
    this.SemEstoque = 3;
};

EnumSituacaoEstoqueProdutoArmazemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Estoque Total", value: this.EstoqueTotal },
            { text: "Estoque Parcial", value: this.EstoqueParcial },
            { text: "Sem Estoque", value: this.SemEstoque }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumSituacaoEstoqueProdutoArmazem = Object.freeze(new EnumSituacaoEstoqueProdutoArmazemHelper());