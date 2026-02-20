var EnumSituacaoEscalaHelper = function () {
    this.Todas = "";
    this.EmCriacao = 1;
    this.AgVeiculos = 2;
    this.Finalizada = 3;
    this.Cancelada = 4;
};

EnumSituacaoEscalaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Veículos", value: this.AgVeiculos },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Em Criação", value: this.EmCriacao },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoEscala = Object.freeze(new EnumSituacaoEscalaHelper());