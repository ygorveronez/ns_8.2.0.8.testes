var EnumTipoAplicacaoColetaEntregaHelper = function () {
    this.Todos = 0;
    this.Entrega = 1;
    this.Coleta = 2;
};

EnumTipoAplicacaoColetaEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entrega", value: this.Entrega },
            { text: "Coleta", value: this.Coleta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Coleta", value: this.Coleta },
            { text: "Entrega", value: this.Entrega }
        ];
    }
}

var EnumTipoAplicacaoColetaEntrega = Object.freeze(new EnumTipoAplicacaoColetaEntregaHelper());