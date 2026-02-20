var EnumTipoChequeHelper = function () {
    this.Todos = "";
    this.Caucao = 1;
    this.Pago = 2;
    this.Recebido = 3;
    this.Repassado = 4;
    this.Emitido = 5;
};

EnumTipoChequeHelper.prototype = {
    obterListaTiposBaixaTituloPagar: function () {
        return [this.Emitido, this.Repassado];
    },
    obterListaTiposBaixaTituloReceber: function () {
        return [this.Caucao, this.Pago, this.Recebido, this.Repassado];
    },
    obterOpcoes: function () {
        return [
            { text: "Caução", value: this.Caucao },
            { text: "Emitido", value: this.Emitido },
            { text: "Pago", value: this.Pago },
            { text: "Recebido", value: this.Recebido },
            { text: "Repassado", value: this.Repassado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCheque = Object.freeze(new EnumTipoChequeHelper());