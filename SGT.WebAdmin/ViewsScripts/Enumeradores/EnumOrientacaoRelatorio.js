var EnumOrientacaoRelatorioHelper = function () {
    this.Retrato = 1;
    this.Paisagem = 2;
};

EnumOrientacaoRelatorioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.OrientacaoRelatorio.Retrato, value: this.Retrato },
            { text: Localization.Resources.Enumeradores.OrientacaoRelatorio.Paisagem, value: this.Paisagem }
        ];
    },
}

var EnumOrientacaoRelatorio = Object.freeze(new EnumOrientacaoRelatorioHelper());