var EnumTipoInteracaoEntregaHelper = function () {
    this.Todas = "";
    this.Mobile = 1;
    this.Manual = 2;
    this.Hibrido = 3;
}

EnumTipoInteracaoEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Mobile", value: this.Mobile },
            { text: "Manual", value: this.Manual },
            { text: "Híbrido", value: this.Hibrido },
        ];
    },
    obterOpcoesConsolidadoEntregas: function () {
        return [
            { text: "Mobile", value: this.Mobile },
            { text: "Manual", value: this.Manual },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaConsolidadoEntregas: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesConsolidadoEntregas());
    }
}

var EnumTipoInteracaoEntrega = Object.freeze(new EnumTipoInteracaoEntregaHelper());
