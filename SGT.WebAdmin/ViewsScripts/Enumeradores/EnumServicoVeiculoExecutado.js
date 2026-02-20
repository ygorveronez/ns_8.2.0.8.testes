var EnumServicoVeiculoExecutadoHelper = function () {
    this.NaoDefinido = 0;
    this.Executado = 1;
    this.NaoExecutado = 2;
};

EnumServicoVeiculoExecutadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Executado", value: this.Executado },
            { text: "Não Executado", value: this.NaoExecutado }
        ];
    }
};

var EnumServicoVeiculoExecutado = Object.freeze(new EnumServicoVeiculoExecutadoHelper());