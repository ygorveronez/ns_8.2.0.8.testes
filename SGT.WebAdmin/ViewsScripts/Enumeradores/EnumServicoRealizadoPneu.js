var EnumServicoRealizadoPneuHelper = function () {
    this.Conserto = 1;
    this.Reforma = 2;
    this.ConsertoEReforma = 3;
}

EnumServicoRealizadoPneuHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Conserto", value: this.Conserto },
            { text: "Reforma", value: this.Reforma },
            { text: "Conserto + Reforma", value: this.ConsertoEReforma }
        ];
    }
}

var EnumServicoRealizadoPneu = Object.freeze(new EnumServicoRealizadoPneuHelper());