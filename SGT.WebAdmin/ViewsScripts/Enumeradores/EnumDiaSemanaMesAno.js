var EnumDiaSemanaMesAnoHelper = function () {
    this.Dia = 1;
    this.Semana = 2;
    this.Mes = 3;
    this.Ano = 4;
};

EnumDiaSemanaMesAnoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.Dia, value: this.Dia },
            { text: Localization.Resources.Gerais.Geral.Semana, value: this.Semana },
            { text: Localization.Resources.Gerais.Geral.Mes, value: this.Mes },
            { text: Localization.Resources.Gerais.Geral.Ano, value: this.Ano }
        ];
    },
}

var EnumDiaSemanaMesAno = Object.freeze(new EnumDiaSemanaMesAnoHelper());