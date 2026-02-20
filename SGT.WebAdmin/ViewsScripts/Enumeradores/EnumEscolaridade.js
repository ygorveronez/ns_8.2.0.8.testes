var EnumEscolaridadeHelper = function () {
    this.SemInstrucaoFormal = 0;
    this.EnsinoFundamental = 1;
    this.EnsinoMedio = 2;
    this.EnsinoSuperior = 3;
    this.PosGraduacao = 4;
    this.Mestrado = 5;
    this.Doutorado = 6;
};

EnumEscolaridadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Escolaridade.SemInstrucaoFormal, value: this.SemInstrucaoFormal },
            { text: Localization.Resources.Enumeradores.Escolaridade.EnsinoFundamental, value: this.EnsinoFundamental },
            { text: Localization.Resources.Enumeradores.Escolaridade.EnsinoMedio, value: this.EnsinoMedio },
            { text: Localization.Resources.Enumeradores.Escolaridade.EnsinoSuperior, value: this.EnsinoSuperior },
            { text: Localization.Resources.Enumeradores.Escolaridade.PosGraduacao, value: this.PosGraduacao },
            { text: Localization.Resources.Enumeradores.Escolaridade.Mestrado, value: this.Mestrado },
            { text: Localization.Resources.Enumeradores.Escolaridade.Doutorado, value: this.Doutorado }
        ];
    }
};

var EnumEscolaridade = Object.freeze(new EnumEscolaridadeHelper());