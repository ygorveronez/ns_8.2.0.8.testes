var EnumTipoAssinaturaVendaDiretaHelper = function () {
    this.Todos = "";
    this.Presencial = 1;
    this.Videoconferencia = 2;
};

EnumTipoAssinaturaVendaDiretaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Presencial", value: this.Presencial },
            { text: "Videoconferência", value: this.Videoconferencia }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoAssinaturaVendaDireta = Object.freeze(new EnumTipoAssinaturaVendaDiretaHelper());