var EnumSituacaoControleVisitaHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Fechado = 2;
};

EnumSituacaoControleVisitaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Fechado", value: this.Fechado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Aberto", value: this.Aberto },
            { text: "Fechado", value: this.Fechado }
        ];
    }
};

var EnumSituacaoControleVisita = Object.freeze(new EnumSituacaoControleVisitaHelper());