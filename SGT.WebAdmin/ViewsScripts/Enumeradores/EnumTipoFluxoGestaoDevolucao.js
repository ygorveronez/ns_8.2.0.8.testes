var EnumTipoFluxoGestaoDevolucaoHelper = function () {
    this.Todos = null;
    this.Normal = 1;
    this.Simples = 2;
};

EnumTipoFluxoGestaoDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Normal", value: this.Normal },
            { text: "Simples", value: this.Simples },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: null },
            { text: "Normal", value: this.Normal },
            { text: "Simples", value: this.Simples },
        ];
    }
};

var EnumTipoFluxoGestaoDevolucao = Object.freeze(new EnumTipoFluxoGestaoDevolucaoHelper());