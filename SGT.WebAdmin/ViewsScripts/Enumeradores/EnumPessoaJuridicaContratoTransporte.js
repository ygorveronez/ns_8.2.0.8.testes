var EnumPessoaJuridicaContratoTransporteHelper = function () {
    this.Todos = "";
    this.Selecione = "";
    this.ULBRUnileverBrasilLtda = 0;
    this.ULBRUnileverBrasilIndustrialLtda = 1;
    this.ULBRUnileverBrasilGeladosLtda = 2;
};

EnumPessoaJuridicaContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "ULBR : Unilever Brasil Ltda", value: this.ULBRUnileverBrasilLtda },
            { text: "ULBR : Unilever Brasil Industrial Ltda", value: this.ULBRUnileverBrasilIndustrialLtda },
            { text: "ULBR : Unilever Brasil Gelados Ltda", value: this.ULBRUnileverBrasilGeladosLtda },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    },
    ObterOpcoesPesquisaIntegracao: function () {
        return [{ text: "Selecione uma opção", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumPessoaJuridicaContratoTransporte = Object.freeze(new EnumPessoaJuridicaContratoTransporteHelper());