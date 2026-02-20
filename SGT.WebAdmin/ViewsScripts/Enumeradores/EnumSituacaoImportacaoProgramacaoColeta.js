var EnumSituacaoImportacaoProgramacaoColetaHelper = function () {
    this.Todos = 0;
    this.EmCriacao = 1;
    this.EmAndamento = 2;
    this.Finalizado = 3;
    this.FalhaNaGeracao = 4;
};

EnumSituacaoImportacaoProgramacaoColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Criação", value: this.EmCriacao },
            { text: "Em Andamento", value: this.EmAndamento },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Falha na Geração", value: this.FalhaNaGeracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoImportacaoProgramacaoColeta = Object.freeze(new EnumSituacaoImportacaoProgramacaoColetaHelper());