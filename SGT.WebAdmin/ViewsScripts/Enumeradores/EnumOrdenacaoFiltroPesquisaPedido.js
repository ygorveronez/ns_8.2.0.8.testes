var EnumOrdenacaoFiltroPesquisaPedidoHelper = function () {
    this.Padrao = 0;
    this.Remetente = 1;
    this.Destinatario = 2;
};

EnumOrdenacaoFiltroPesquisaPedidoHelper.prototype.obterOpcoes = function () {
    return [
        { text: "Padrao", value: this.Padrao },
        { text: "Destinatario", value: this.Destinatario },
        { text: "Remetente", value: this.Remetente }
    ];
};

var EnumOrdenacaoFiltroPesquisaPedido = Object.freeze(new EnumOrdenacaoFiltroPesquisaPedidoHelper());