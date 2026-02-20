var EnumImagemConfirmacaoHelper = function () {
    this.Todos = "";
    this.Imagem = 1;
    this.Confirmacao = 2;
};

EnumImagemConfirmacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Imagem", value: this.Imagem },
            { text: "Confirmação", value: this.Confirmacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumImagemConfirmacao = Object.freeze(new EnumImagemConfirmacaoHelper());