var EnumTipoServicoVeiculoHelper = function () {
    this.Ambos = 0;
    this.PorKM = 1;
    this.PorDia = 2;
    this.PorHorimetro = 3;
    this.Todos = 4;
    this.PorHorimetroDia = 5;
    this.Nenhum = 6;
};

EnumTipoServicoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por KM e dia", value: this.Ambos },
            { text: "Por KM", value: this.PorKM },
            { text: "Por dia", value: this.PorDia },
            { text: "Por Horímetro", value: this.PorHorimetro },
            { text: "Por Horímetro e dia", value: this.PorHorimetroDia },
            { text: "Nenhum", value: this.Nenhum },
            { text: "Todos", value: this.Todos }
        ];
    }
};

var EnumTipoServicoVeiculo = Object.freeze(new EnumTipoServicoVeiculoHelper());