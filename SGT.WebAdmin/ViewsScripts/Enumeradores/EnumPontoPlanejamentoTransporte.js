var EnumPontoPlanejamentoTransporteHelper = function () {
    this.Selecione = "";
    this.BR01 = 1;
    this.BR02 = 2;
    this.BR04 = 3;
};

EnumPontoPlanejamentoTransporteHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: "BR01 - Unilever Brasil", value: this.BR01 },
            { text: "BR02 - Unilever Brasil Industrial Ltda", value: this.BR02 },
            { text: "BR04 – Com.Alim.ICE VA", value: this.BR04 },
        ];

        return arrayOpcoes;
    },
    obterOpcoesPesquisaIntegracaoLBC: function () {
        return [{ text: "Selecione uma opção", value: "" }].concat(this.obterOpcoes());
    }
};

var EnumPontoPlanejamentoTransporte = Object.freeze(new EnumPontoPlanejamentoTransporteHelper());