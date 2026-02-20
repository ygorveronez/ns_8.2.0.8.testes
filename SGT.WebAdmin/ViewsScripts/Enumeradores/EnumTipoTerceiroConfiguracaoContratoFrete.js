var EnumTipoTerceiroConfiguracaoContratoFreteHelper = function () {
    this.Todos = "";
    this.PorPessoa = 1;
    this.PorTipoDeOperacao = 2;
    this.PorTipoDeTerceiro = 3;
};

EnumTipoTerceiroConfiguracaoContratoFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por Pessoa (Transp. Terceiro)", value: this.PorPessoa },
            { text: "Por Tipo de Operação", value: this.PorTipoDeOperacao },
            { text: "Por Tipo de Terceiro", value: this.PorTipoDeTerceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTerceiroConfiguracaoContratoFrete = Object.freeze(new EnumTipoTerceiroConfiguracaoContratoFreteHelper());