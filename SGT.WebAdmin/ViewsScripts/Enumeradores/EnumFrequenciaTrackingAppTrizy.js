var EnumFrequenciaTrackingAppTrizyHelper = function () {
    this.VeryLow = 0;
    this.Low = 1;
    this.Medium = 2;
    this.High = 3;
    this.VeryHigh = 4;
};

EnumFrequenciaTrackingAppTrizyHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Muito baixo", value: this.VeryLow },
            { text: "Baixo", value: this.Low },
            { text: "Médio", value: this.Medium },
            { text: "Alto", value: this.High },
            { text: "Muito alto", value: this.VeryHigh },
        ];
    }
};

var EnumFrequenciaTrackingAppTrizy = Object.freeze(new EnumFrequenciaTrackingAppTrizyHelper());