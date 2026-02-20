var EnumSituacaoTituloHelper = function () {
    this.Todos = 0;
    this.EmAberto = 1;
    this.Atrazado = 2;
    this.Quitado = 3;
    this.Cancelado = 4;
    this.EmNegociacao = 5;
    this.Bloqueado = 6;
};

EnumSituacaoTituloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Aberto", value: this.EmAberto },
            { text: "Quitado", value: this.Quitado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoTitulo = Object.freeze(new EnumSituacaoTituloHelper());