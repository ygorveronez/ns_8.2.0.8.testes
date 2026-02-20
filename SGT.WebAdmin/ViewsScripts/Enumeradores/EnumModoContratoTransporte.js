var EnumModoContratoTransporteHelper = function () {
    this.BRAereo = 0;
    this.BRFerroviário = 1;
    this.BRMaritimo = 2;
    this.BRRodoviario = 3;
    this.BRWarehouse = 4;
    this.Selecione = "";
};

EnumModoContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "BR Aéreo", value: this.BRAereo },
            { text: "BR Ferroviário", value: this.BRFerroviário },
            { text: "BR Marítimo", value: this.BRMaritimo },
            { text: "BR Rodoviário", value: this.BRRodoviario },
            { text: "BR Warehouse", value: this.BRWarehouse },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    },
    ObterOpcoesPesquisaIntegracao: function () {
        return [{ text: "Selecione uma opção", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumModoContratoTransporte = Object.freeze(new EnumModoContratoTransporteHelper());