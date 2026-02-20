var EnumTipoVisualizacaoCapacidadeJanelaHelper = function () {
    this.Volume = 1;
    this.Cubagem = 2;
};

EnumTipoVisualizacaoCapacidadeJanelaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Volume", value: this.Volume },
            { text: "Cubagem", value: this.Cubagem }
        ];
    },
};

var EnumTipoVisualizacaoCapacidadeJanela = Object.freeze(new EnumTipoVisualizacaoCapacidadeJanelaHelper());