var EnumTipoCombustivelHelper = function () {
    this.Todos = "";
    this.Gasolina = "G";
    this.DieselS500 = "D";
    this.DieselS10 = "I";
    this.Etanol = "E";
    this.GasNatural = "N";
    this.Diesel = "S";
    this.Outros = "O";
};

EnumTipoCombustivelHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCombustivel.Gasolina, value: this.Gasolina },
            { text: Localization.Resources.Enumeradores.TipoCombustivel.DieselQuinhentos, value: this.DieselS500 },
            { text: Localization.Resources.Enumeradores.TipoCombustivel.DieselDez, value: this.DieselS10 },
            { text: Localization.Resources.Enumeradores.TipoCombustivel.Etanol, value: this.Etanol },
            { text: Localization.Resources.Enumeradores.TipoCombustivel.GasNatural, value: this.GasNatural },
            { text: Localization.Resources.Enumeradores.TipoCombustivel.Diesel, value: this.Diesel },
            { text: Localization.Resources.Enumeradores.TipoCombustivel.Outros, value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCombustivel = Object.freeze(new EnumTipoCombustivelHelper());