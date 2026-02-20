var EnumRequisitoHelper = function () {
    this.Desejavel = 0;
    this.Indispensavel = 1;
};

EnumRequisitoHelper.prototype = {
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
    }
};

var EnumRequisito = Object.freeze(new EnumRequisitoHelper());