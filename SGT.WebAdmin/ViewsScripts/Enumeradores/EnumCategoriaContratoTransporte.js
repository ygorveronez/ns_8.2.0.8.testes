var EnumCategoriaContratoTransporteHelper = function () {
    this.Todos = "";
    this.Selecione = "";
    this.Inbound = 0;
    this.PrimaryTransport = 1;
    this.SecundaryTransport = 2;
};

EnumCategoriaContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Inbound", value: this.Inbound },
            { text: "Primary transport", value: this.PrimaryTransport },
            { text: "Secondary transport", value: this.SecundaryTransport },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    },
    ObterOpcoesPesquisaIntegracao: function () {
        return [{ text: "Selecione uma opção", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumCategoriaContratoTransporte = Object.freeze(new EnumCategoriaContratoTransporteHelper());