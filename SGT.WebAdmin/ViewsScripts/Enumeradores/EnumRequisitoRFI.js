let EnumRequisitoRFIHelper = function () {
    this.Desejavel = 0;
    this.Indispensavel = 1;
};

EnumRequisitoRFIHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Desejavel: return "Desejável";
            case this.Indispensavel: return "Indispensável";
            default: return "";
        }
    },

    obterOpcoes: function () {
        return [
            { text: "Desejável", value: this.Desejavel },
            { text: "Indispensável", value: this.Indispensavel }
        ];
    },

    obterValor: function (valor) {
        switch (valor) {
            case "Desejável": return this.Desejavel;
            case "Indispensável": return this.Indispensavel;
            default: return "";
        }
    }
};

let EnumRequisitoRFI = Object.freeze(new EnumRequisitoRFIHelper());