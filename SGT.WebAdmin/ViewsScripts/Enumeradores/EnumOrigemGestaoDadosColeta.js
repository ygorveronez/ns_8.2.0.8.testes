var EnumOrigemGestaoDadosColetaHelper = function () {
    this.Todas = "";
    this.Embarcador = 0;
    this.Transportador = 1;
    this.Motorista = 2;
};

EnumOrigemGestaoDadosColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Embarcador", value: this.Embarcador },
            { text: "Motorista", value: this.Motorista },
            { text: "Transportador", value: this.Transportador }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumOrigemGestaoDadosColeta = Object.freeze(new EnumOrigemGestaoDadosColetaHelper());
