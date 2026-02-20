var EnumSituacaoPgtoCanhotoHelper = function () {
    this.Todas = 0;
    this.Pendente = 1;
    this.Liberado = 2;
    this.Rejeitado = 3;
};

EnumSituacaoPgtoCanhotoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Liberado", value: this.Liberado },
            { text: "Rejeitado", value: this.Rejeitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPgtoCanhoto = Object.freeze(new EnumSituacaoPgtoCanhotoHelper());