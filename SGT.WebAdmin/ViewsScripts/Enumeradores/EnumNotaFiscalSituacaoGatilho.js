var EnumNotaFiscalSituacaoGatilhoHelper = function () {
    this.SemGatilho = 0;
    this.ConfirmacaoEntregaNota = 1;
    this.RejeicaoEntregaNota = 2;
    this.RetificarEntrega = 3;
};

EnumNotaFiscalSituacaoGatilhoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem gatilho", value: this.SemGatilho },
            { text: "Confirmação entrega nota", value: this.ConfirmacaoEntregaNota },
            { text: "Rejeição entrega nota", value: this.RejeicaoEntregaNota },
            { text: "Retificar entrega nota", value: this.RetificarEntrega },
        ];
    }
};

var EnumNotaFiscalSituacaoGatilho = Object.freeze(new EnumNotaFiscalSituacaoGatilhoHelper());