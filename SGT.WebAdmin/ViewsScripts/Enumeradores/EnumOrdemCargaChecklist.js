var EnumOrdemCargaChecklistHelper = function () {
    this.UltimaCarga = 1;
    this.PenultimaCarga = 2;
    this.AntepenultimaCarga = 3;
};

EnumOrdemCargaChecklistHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Última Carga", value: this.UltimaCarga },
            { text: "Penúltima Carga", value: this.PenultimaCarga },
            { text: "Antepenúltima Carga", value: this.AntepenultimaCarga }
        ];
    },
    obterDescricao: function (value) {
        if (value == this.UltimaCarga)
            return "Última Carga";
        if (value == this.PenultimaCarga)
            return "Penúltima Carga";
        if (value == this.AntepenultimaCarga)
            return "Antepenúltima Carga";
    }
}

var EnumOrdemCargaChecklist = Object.freeze(new EnumOrdemCargaChecklistHelper());