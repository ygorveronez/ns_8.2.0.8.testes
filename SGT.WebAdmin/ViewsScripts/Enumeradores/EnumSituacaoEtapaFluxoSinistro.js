var EnumSituacaoEtapaFluxoSinistroHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
};

EnumSituacaoEtapaFluxoSinistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoEtapaFluxoSinistro = Object.freeze(new EnumSituacaoEtapaFluxoSinistroHelper());