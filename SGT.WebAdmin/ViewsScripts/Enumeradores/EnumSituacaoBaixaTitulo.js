var EnumSituacaoBaixaTituloHelper = function () {
    this.Iniciada = 1;
    this.EmNegociacao = 2;
    this.Finalizada = 3;
    this.Cancelada = 4;
    this.EmGeracao = 5;
    this.EmFinalizacao = 6;
};

EnumSituacaoBaixaTituloHelper.prototype.ObterOpcoes = function () {
    return [
        { text: "Iniciada", value: this.Iniciada },
        { text: "Em Negociação", value: this.EmNegociacao },
        { text: "Finalizada", value: this.Finalizada },
        { text: "Cancelada", value: this.Cancelada },
        { text: "Em Geração", value: this.EmGeracao },
        { text: "Em Finalização", value: this.EmFinalizacao }
    ];
};

EnumSituacaoBaixaTituloHelper.prototype.ObterOpcoesPesquisa = function () {
    return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
};

var EnumSituacaoBaixaTitulo = Object.freeze(new EnumSituacaoBaixaTituloHelper());