var EnumRegimeLimpezaHelper = function () {
    this.Nenhum = 0;
    this.LimpezaASeco = 1;
    this.LimpezaComAgua = 2;
    this.LimpezaComAguaEAgenteDeLimpeza = 3;
    this.LimpezaEDesinfeccao = 4;
};

EnumRegimeLimpezaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Regime A – Limpeza a seco", value: this.LimpezaASeco },
            { text: "Regime B – Limpeza com Água", value: this.LimpezaComAgua },
            { text: "Regime C – Limpeza com água e um agente de limpeza", value: this.LimpezaComAguaEAgenteDeLimpeza },
            { text: "Regime D – Limpeza e Desinfecção", value: this.LimpezaEDesinfeccao },
        ];
    },
}

var EnumRegimeLimpeza = Object.freeze(new EnumRegimeLimpezaHelper());