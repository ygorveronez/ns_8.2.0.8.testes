var EnumSituacaoProgramacaoLogisticaHelper = function () {
    this.EmCarga = 1;
    this.Vazio = 2;
    this.NaoVazio = 3;
    this.Todos = 0;
};

EnumSituacaoProgramacaoLogisticaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Carga", value: this.EmCarga },
            { text: "Vazio", value: this.Vazio },
            { text: "Não Vazio", value: this.NaoVazio },
            { text: "Todos", value: this.Todos }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoProgramacaoLogistica = Object.freeze(new EnumSituacaoProgramacaoLogisticaHelper());