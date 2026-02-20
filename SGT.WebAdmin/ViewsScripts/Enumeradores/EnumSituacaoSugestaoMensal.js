var EnumSituacaoSugestaoMensalHelper = function () {
    this.Todos = -1;
    this.EmAnaliseTransportador = 0;
    this.EmAnaliseEmbarcador = 1;
    this.Confirmado = 2;
    this.Rejeitado = 3;
};

EnumSituacaoSugestaoMensalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Análise Transportador", value: this.EmAnaliseTransportador },
            { text: "Em Análise Embarcador", value: this.EmAnaliseEmbarcador },
            { text: "Confirmado", value: this.Confirmado },
            { text: "Rejeitado", value: this.Rejeitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoSugestaoMensal = Object.freeze(new EnumSituacaoSugestaoMensalHelper());