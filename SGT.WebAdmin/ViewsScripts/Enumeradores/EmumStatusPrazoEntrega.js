var EnumStatusPrazoEntregaHelper = function() {
    this.Todos = "";
    this.NoPrazo = 0;
    this.Antecipado = 1;
    this.Atrasado = 2;
    
};


EnumStatusPrazoEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "No Prazo", value: this.NoPrazo },
            { text: "Antecipado", value: this.Antecipado },
            { text: "Atrazado", value: this.Atrasado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

};


var EnumStatusPrazoEntrega = Object.freeze(new EnumStatusPrazoEntregaHelper());