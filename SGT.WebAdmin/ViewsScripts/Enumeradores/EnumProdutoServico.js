var EnumProdutoServicoHelper = function () {
    this.Todos = 0;
    this.Produto = 1;
    this.Servico = 2;
};

EnumProdutoServicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Produto", value: this.Produto },
            { text: "Serviço", value: this.Servico }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumProdutoServico = Object.freeze(new EnumProdutoServicoHelper());