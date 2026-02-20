var EnumNovasRodadasBaselineTargetHelper = function () {
    this.Baseline = 1;
    this.MenorValor = 2;
    this.ValorAlvo = 3;
};

EnumNovasRodadasBaselineTargetHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "% Baseline", value: this.Baseline },
            { text: "% Menor Valor", value: this.MenorValor },
            { text: "Valor Alvo", value: this.ValorAlvo },
        ];
    }
};

var EnumNovasRodadasBaselineTarget = Object.freeze(new EnumNovasRodadasBaselineTargetHelper());