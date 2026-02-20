var EnumStatusChequeHelper = function () {
    this.Todos = "";
    this.Cancelado = 1;
    this.Compensado = 2;
    this.Pendente = 3;
    this.Normal = 4;
    this.Devolvido = 5;
    this.Depositado = 6;
    this.SemFundos = 7;
    this.Reapresentado = 8;
    this.Repassado = 9;
    this.Sustado = 10;
    this.Disponivel = 11;
};

EnumStatusChequeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Cancelado", value: this.Cancelado },
            { text: "Compensado", value: this.Compensado },
            { text: "Pendente", value: this.Pendente },
            { text: "Normal", value: this.Normal },
            { text: "Devolvido", value: this.Devolvido },
            { text: "Depositado", value: this.Depositado },
            { text: "Sem Fundos", value: this.SemFundos },
            { text: "Reapresentado", value: this.Reapresentado },
            { text: "Repassado", value: this.Repassado },
            { text: "Sustado", value: this.Sustado },
            { text: "Disponível", value: this.Disponivel }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusCheque = Object.freeze(new EnumStatusChequeHelper());