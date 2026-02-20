var EnumStatusControleNotaDevolucaoHelper = function () {
    this.Todos = 0;
    this.AgNotaFiscal = 1;
    this.ComNotaFiscal = 2;
    this.Conferido = 3;
    this.Rejeitado = 4;
    this.ComChaveNotaFiscal = 5;
};

EnumStatusControleNotaDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Nota Fiscal", value: this.AgNotaFiscal },
            { text: "Com Chave Nota Fiscal", value: this.ComChaveNotaFiscal },
            { text: "Com Nota Fiscal", value: this.ComNotaFiscal },
            { text: "Conferido", value: this.Conferido },
            { text: "Rejeitado", value: this.Rejeitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusControleNotaDevolucao = Object.freeze(new EnumStatusControleNotaDevolucaoHelper());