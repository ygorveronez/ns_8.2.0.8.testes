var EnumEtapaAjusteTabelaFreteHelper = function () {
    this.Todas = 9;
    this.Criacao = 0;
    this.AgAprovacao = 1;
    this.AgIntegracao = 2;
    this.Finalizada = 3;
};

EnumEtapaAjusteTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Criação", value: this.Criacao },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumEtapaAjusteTabelaFrete = Object.freeze(new EnumEtapaAjusteTabelaFreteHelper());