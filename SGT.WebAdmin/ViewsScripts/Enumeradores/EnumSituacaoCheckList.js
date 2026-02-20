var EnumSituacaoCheckListHelper = function () {
    this.Todas = "";
    this.Aberto = 1;
    this.Finalizado = 2;
    this.Rejeitado = 3;
};

EnumSituacaoCheckListHelper.prototype = {
    isPermiteEdicao: function (situacao) {
        return (situacao == this.Aberto);
    },
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Rejeitado", value: this.Rejeitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoCheckList = Object.freeze(new EnumSituacaoCheckListHelper());