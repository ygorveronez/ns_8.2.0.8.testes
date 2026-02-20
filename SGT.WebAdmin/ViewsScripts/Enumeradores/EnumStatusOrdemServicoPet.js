var EnumStatusOrdemServicoPetHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.EmAndamento = 2;
    this.Finalizado = 3;
};

EnumStatusOrdemServicoPetHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Em andamento", value: this.EmAndamento },
            { text: "Finalizado", value: this.Finalizado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusOrdemServicoPet = Object.freeze(new EnumStatusOrdemServicoPetHelper());