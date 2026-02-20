var EnumSituacaoComprovanteCargaHelper = function () {
    this.Pendente = 0;
    this.Recebido = 1;
    this.Justificado = 2;
    this.Todas = "";
};

EnumSituacaoComprovanteCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Recebido", value: this.Recebido },
            { text: "Justificado", value: this.Justificado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoComprovanteCarga = Object.freeze(new EnumSituacaoComprovanteCargaHelper());