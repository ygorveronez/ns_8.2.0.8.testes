var EnumBloqueioDocumentoEntradaHelper = function () {
    this.SemBloqueio = 0;
    this.SemOrdemServico = 1;
    this.SemOrdemCompra = 2;
    this.SemOrdemServicoESemOrdemCompra = 3;
    this.SemOrdemServicoOuSemOrdemCompra = 4;
};

EnumBloqueioDocumentoEntradaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Bloqueio", value: this.SemBloqueio },
            { text: "Sem Ordem de Serviço", value: this.SemOrdemServico },
            { text: "Sem Ordem de Compra", value: this.SemOrdemCompra },
            { text: "Sem Ordem de Serviço e sem Ordem de Compra", value: this.SemOrdemServicoESemOrdemCompra },
            { text: "Sem Ordem de Serviço ou sem Ordem de Compra", value: this.SemOrdemServicoOuSemOrdemCompra }
        ];
    }
};

var EnumBloqueioDocumentoEntrada = Object.freeze(new EnumBloqueioDocumentoEntradaHelper());