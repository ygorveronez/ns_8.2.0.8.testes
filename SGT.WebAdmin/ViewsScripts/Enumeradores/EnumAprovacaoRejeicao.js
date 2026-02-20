var EnumAprovacaoRejeicaoHelper = function () {
    this.Todos = "";
    this.Rejeicao = 0;
    this.Aprovacao = 1;
};

EnumAprovacaoRejeicaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aprovação", value: this.Aprovacao },
            { text: "Rejeição", value: this.Rejeicao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumAprovacaoRejeicao = Object.freeze(new EnumAprovacaoRejeicaoHelper());