var EnumNivelToleranciaValorHelper = function () {
    this.Todos = "";
    this.NaoAceitaDivergencia = 0;
    this.AceitaValorDivergente = 1;
    this.AceitaValorMenor = 2;
};

EnumNivelToleranciaValorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.NivelToleranciaValor.NaoAceitaDivergencias, value: this.NaoAceitaDivergencia },
            { text: Localization.Resources.Enumeradores.NivelToleranciaValor.AceitaValoresDivergentes, value: this.AceitaValorDivergente },
            { text: Localization.Resources.Enumeradores.NivelToleranciaValor.AceitaValoresMenores, value: this.AceitaValorMenor }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.NivelToleranciaValor.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumNivelToleranciaValor = Object.freeze(new EnumNivelToleranciaValorHelper());