let EnumSituacaoImportacaoCTeEmitidoForaEmbarcadorHelper = function () {
    this.Todas = 0;
    this.Pendente = 1;
    this.Processando = 2;
    this.Sucesso = 3;
    this.Erro = 4;
    this.Cancelado = 5;
};

EnumSituacaoImportacaoCTeEmitidoForaEmbarcadorHelper.prototype = {
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

let EnumSituacaoImportacaoCTeEmitidoForaEmbarcador = Object.freeze(new EnumSituacaoImportacaoCTeEmitidoForaEmbarcadorHelper());