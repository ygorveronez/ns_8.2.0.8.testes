var EnumTipoImpressaoPedidoPrestacaoServicoHelper = function () {
    this.PrestacaoComViaCliente = 1;
    this.PrestacaoSemViaCliente = 2;
};

EnumTipoImpressaoPedidoPrestacaoServicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Prestação com Via Cliente", value: this.PrestacaoComViaCliente },
            { text: "Prestação sem Via Cliente", value: this.PrestacaoSemViaCliente }
        ];
    }
};

var EnumTipoImpressaoPedidoPrestacaoServico = Object.freeze(new EnumTipoImpressaoPedidoPrestacaoServicoHelper());