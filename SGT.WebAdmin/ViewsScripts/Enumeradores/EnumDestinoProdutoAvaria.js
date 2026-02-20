var EnumDestinoProdutoAvariaHelper = function () {
    this.Descartada = 1;
    this.Vendida = 2;
    this.DescontadaMotorista = 3;
    this.DevolvidaCliente = 4;
    this.DescontoFatura = 5;
};

EnumDestinoProdutoAvariaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Descartada", value: this.Descartada },
            { text: "Vendida", value: this.Vendida },
            { text: "Descontada do Motorista", value: this.DescontadaMotorista },
            { text: "Devolvida ao Cliente", value: this.DevolvidaCliente },
            { text: "Desconto em Fatura", value: this.DescontoFatura },
        ];
    }
};

var EnumDestinoProdutoAvaria = Object.freeze(new EnumDestinoProdutoAvariaHelper());