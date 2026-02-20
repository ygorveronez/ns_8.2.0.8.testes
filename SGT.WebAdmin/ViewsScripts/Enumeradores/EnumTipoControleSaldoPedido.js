var EnumTipoControleSaldoPedidoHelper = function () {
    this.Todos = -1;
    this.Peso = 0;
    this.Pallet = 1;
    this.CarregamentoUnico = 9;
};

EnumTipoControleSaldoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carregamento único", value: this.CarregamentoUnico },
            { text: "Pallet", value: this.Pallet },
            { text: "Peso", value: this.Peso }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoControleSaldoPedido = Object.freeze(new EnumTipoControleSaldoPedidoHelper());
