var EnumTipoEnvioTransporteMaritimoHelper = function () {
    this.Todos = -1;
    this.TON = 1;
    this.FCL = 2;
};

EnumTipoEnvioTransporteMaritimoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "1 - TON", value: this.TON },
            { text: "2 - FCL", value: this.FCL }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoEnvioTransporteMaritimo = Object.freeze(new EnumTipoEnvioTransporteMaritimoHelper());