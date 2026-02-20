var EnumIndicadorNegociavelHelper = function () {
    this.Todos = "";
    this.NaoNegociavel = "0";
    this.Negociavel = "1";
};

EnumIndicadorNegociavelHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.IndicadorNegociavel.NaoNegociavel, value: this.NaoNegociavel },
            { text: Localization.Resources.Enumeradores.IndicadorNegociavel.Negociavel, value: this.Negociavel }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.IndicadorNegociavel.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIndicadorNegociavel = Object.freeze(new EnumIndicadorNegociavelHelper());