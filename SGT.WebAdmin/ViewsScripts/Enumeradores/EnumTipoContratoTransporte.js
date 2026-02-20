var EnumTipoContratoTransporteHelper = function () {
    this.CTC = 0;
    this.NDA = 1;
    this.EkaterraTeaLogistics = 2;
    this.STC = 3;
    this.UPA = 4;
};

EnumTipoContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "CTC", value: this.CTC },
            { text: "NDA", value: this.NDA },
            { text: "Ekaterra Tea Logistics", value: this.EkaterraTeaLogistics },
            { text: "STC", value: this.STC },
            { text: "UPA", value: this.UPA },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumTipoContratoTransporte = Object.freeze(new EnumTipoContratoTransporteHelper());