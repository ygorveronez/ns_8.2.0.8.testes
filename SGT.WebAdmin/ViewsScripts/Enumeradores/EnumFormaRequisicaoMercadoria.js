var EnumFormaRequisicaoMercadoriaHelper = function () {
    this.GerarPeloEstoque = 0;
    this.Estoque = 1;
    this.Compra = 2;
};

EnumFormaRequisicaoMercadoriaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Gerar pelo estoque", value: this.GerarPeloEstoque },
            { text: "Estoque", value: this.Estoque },
            { text: "Compra", value: this.Compra }
        ];
    }
};

var EnumFormaRequisicaoMercadoria = Object.freeze(new EnumFormaRequisicaoMercadoriaHelper());