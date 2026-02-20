var EnumTipoTransporteDadosMaritimosHelper = function () {
    this.Todos = -1;
    this.NaoDefinido = 0;
    this.Cheio = 1;
    this.Picado = 2;
    this.Solto = 2;
};

EnumTipoTransporteDadosMaritimosHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Cheio", value: this.Cheio },
            { text: "Picado", value: this.Picado },
            { text: "Solto", value: this.Solto }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTransporteDadosMaritimos = Object.freeze(new EnumTipoTransporteDadosMaritimosHelper());