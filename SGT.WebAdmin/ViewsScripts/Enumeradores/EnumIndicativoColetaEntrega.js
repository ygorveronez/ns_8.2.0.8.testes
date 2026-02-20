var EnumIndicativoColetaEntregaHelper = function () {
    this.Todos = "";
    this.NaoInformado = 0;
    this.Coleta = 1;
    this.Entrega = 2;
};

EnumIndicativoColetaEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não informado", value: this.NaoInformado },
            { text: "Coleta", value: this.Coleta },
            { text: "Entrega", value: this.Entrega }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIndicativoColetaEntrega = Object.freeze(new EnumIndicativoColetaEntregaHelper());