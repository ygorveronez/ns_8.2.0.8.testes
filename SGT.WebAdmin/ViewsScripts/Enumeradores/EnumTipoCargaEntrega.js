var EnumTipoCargaEntregaHelper = function () {
    this.Todos = 0;
    this.Entrega = 1;
    this.Coleta = 2;
    this.Fronteira = 3;
};

EnumTipoCargaEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entrega", value: this.Entrega },
            { text: "Coleta", value: this.Coleta },
            { text: "Fronteira", value: this.Fronteira }
        ];
    },
    obterOpcoesAreaContainer: function () {
        return [
            { text: "Entrega", value: this.Entrega },
            { text: "Coleta", value: this.Coleta }
        ];
    },
    obterOpcoesPesquisaAreaContainer: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoesAreaContainer());
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCargaEntrega = Object.freeze(new EnumTipoCargaEntregaHelper());