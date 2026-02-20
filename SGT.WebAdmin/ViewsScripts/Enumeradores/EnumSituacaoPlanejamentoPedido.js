var EnumSituacaoPlanejamentoPedidoHelper = function () {
    this.Todas = "";
    this.Pendente = 0;
    this.EntregueAoMotorista = 1;
    this.MotoristaNoLocalCarregamento = 2;
    this.Problema = 3;
};

EnumSituacaoPlanejamentoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            //{ text: "Entregue ao Motorista", value: this.EntregueAoMotorista },
            { text: "CT-e Emitido", value: this.MotoristaNoLocalCarregamento },
            { text: "Pendente", value: this.Pendente },
            { text: "OK", value: this.Problema }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPlanejamentoPedido = Object.freeze(new EnumSituacaoPlanejamentoPedidoHelper());