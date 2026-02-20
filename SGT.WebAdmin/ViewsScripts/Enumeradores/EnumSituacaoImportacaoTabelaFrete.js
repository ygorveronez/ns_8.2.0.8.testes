var EnumSituacaoImportacaoTabelaFreteHelper = function () {
    this.Todas = 6;
    this.Pendente = 0;
    this.Processando = 1;
    this.Sucesso = 2;
    this.Erro = 3;
    this.Cancelado = 4;
};

EnumSituacaoImportacaoTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Processando", value: this.Processando },
            { text: "Sucesso", value: this.Sucesso },
            { text: "Erro", value: this.Erro },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoImportacaoTabelaFrete = Object.freeze(new EnumSituacaoImportacaoTabelaFreteHelper());