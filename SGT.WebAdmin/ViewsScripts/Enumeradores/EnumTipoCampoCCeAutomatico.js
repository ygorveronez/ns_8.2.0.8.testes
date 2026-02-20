var EnumTipoCampoCCeAutomaticoHelper = function () {
    this.Nenhum = 0;
    this.Booking = 1;
    this.Navio = 2;
    this.PortoPassagem = 3;
};

EnumTipoCampoCCeAutomaticoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Booking", value: this.Booking },
            { text: "Navio", value: this.Navio },
            { text: "Porto de Passagem", value: this.PortoPassagem },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoCampoCCeAutomatico = Object.freeze(new EnumTipoCampoCCeAutomaticoHelper());