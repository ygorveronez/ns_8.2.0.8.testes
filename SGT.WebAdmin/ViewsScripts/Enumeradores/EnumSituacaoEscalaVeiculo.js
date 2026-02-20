var EnumSituacaoEscalaVeiculoHelper = function () {
    this.Todas = "";
    this.EmEscala = 1;
    this.Suspenso = 2;
};

EnumSituacaoEscalaVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Escala", value: this.EmEscala },
            { text: "Suspenso", value: this.Suspenso },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoEscalaVeiculo = Object.freeze(new EnumSituacaoEscalaVeiculoHelper());