var EnumSituacaoPreCTeHelper = function () {
    this.Todos = "";
    this.Pendente = 1;
    this.CTeRecebido = 2;
}

EnumSituacaoPreCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "CT-e Recebido", value: this.CTeRecebido }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
}

var EnumSituacaoPreCTe = Object.freeze(new EnumSituacaoPreCTeHelper());