var EnumTipoRedespachoHelper = function () {
    this.Todos = "";
    this.Redespacho = 1;
    this.Reentrega = 2;
}

EnumTipoRedespachoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Redespacho", value: this.Redespacho },
            { text: "Reentrega", value: this.Reentrega },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoRedespacho = Object.freeze(new EnumTipoRedespachoHelper());