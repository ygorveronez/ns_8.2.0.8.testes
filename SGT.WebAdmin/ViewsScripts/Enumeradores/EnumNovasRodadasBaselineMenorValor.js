var EnumNovasRodadasBaselineMenorValorHelper = function () {
    this.Baseline = 1;
    this.MenorValor = 2;
};

EnumNovasRodadasBaselineMenorValorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "% Baseline", value: this.Baseline },
            { text: "% Menor Valor", value: this.MenorValor },
        ];
    }
};

var EnumNovasRodadasBaselineMenorValor = Object.freeze(new EnumNovasRodadasBaselineMenorValorHelper());