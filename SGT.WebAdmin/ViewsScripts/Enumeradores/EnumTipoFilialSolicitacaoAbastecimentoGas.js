var EnumTipoFilialSolicitacaoAbastecimentoGasHelper = function () {
    this.Todos = 0;
    this.Supridora = 1;
    this.Satelite = 2;
}

EnumTipoFilialSolicitacaoAbastecimentoGasHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        opcoes.push({ text: "Todos", value: this.Todos });
        opcoes.push({ text: "Supridora", value: this.Supridora });
        opcoes.push({ text: "Satélite", value: this.Satelite });
        
        return opcoes;
    }
}

var EnumTipoFilialSolicitacaoAbastecimentoGas = Object.freeze(new EnumTipoFilialSolicitacaoAbastecimentoGasHelper());