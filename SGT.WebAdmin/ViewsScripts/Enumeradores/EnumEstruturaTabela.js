var EnumEstruturaTabelaHelper = function () {
    this.Todos = "";
    this.Selecione = "";
    this.CustoFixo = 0;
    this.TarifaFlat = 1;
    this.TarifasCustoUnidade = 2;
};

EnumEstruturaTabelaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Custo fixo", value: this.CustoFixo },
            { text: "Tarifa flat", value: this.TarifaFlat },
            { text: "Tarifas - Custo por unidade", value: this.TarifasCustoUnidade },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.obterOpcoes());
    },
    obterOpcoesIntegracaoLBC: function () {
        return [{ text: "Selecione uma opção", value: "" }].concat(this.obterOpcoes());
    }
}

var EnumEstruturaTabela = Object.freeze(new EnumEstruturaTabelaHelper());