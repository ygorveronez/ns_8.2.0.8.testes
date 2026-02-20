var EnumSituacaoAlcadaRegraHelper = function () {
    this.Todas = "";
    this.Pendente = "0";
    this.Aprovada = "1";
    this.Rejeitada = "2";
};

EnumSituacaoAlcadaRegraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aprovada", value: this.Aprovada },
            { text: "Pendente", value: this.Pendente },
            { text: "Rejeitada", value: this.Rejeitada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoAlcadaRegra = Object.freeze(new EnumSituacaoAlcadaRegraHelper());