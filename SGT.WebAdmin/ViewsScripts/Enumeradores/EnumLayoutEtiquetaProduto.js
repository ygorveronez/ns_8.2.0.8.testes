var EnumLayoutEtiquetaProdutoHelper = function () {
    this.QrCode = 1;
    this.CodigoBarras = 2;
};

EnumLayoutEtiquetaProdutoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: 'QrCode', value: this.QrCode },
            { text: 'Código De Barras', value: this.CodigoBarras },
        ];
    },    
};

var EnumLayoutEtiquetaProduto = Object.freeze(new EnumLayoutEtiquetaProdutoHelper());
