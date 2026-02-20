var EnumTipoGestaoDadosColetaHelper = function () {
    this.Todos = "";
    this.DadosNfe = 0;
    this.DadosTransporte = 1;
};

EnumTipoGestaoDadosColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dados da NF-e", value: this.DadosNfe },
            { text: "Dados de Transporte", value: this.DadosTransporte }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoGestaoDadosColeta = Object.freeze(new EnumTipoGestaoDadosColetaHelper());
