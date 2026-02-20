var EnumTipoRotaSemPararHelper = function () {
    this.Todos = "";
    this.RotaTemporaria = 0;
    this.RotaFixa = 1;
}

EnumTipoRotaSemPararHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoRotaSemParar.RotaTemporaria, value: this.RotaTemporaria },
            { text: Localization.Resources.Enumeradores.TipoRotaSemParar.RotaFixa, value: this.RotaFixa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRotaSemParar = Object.freeze(new EnumTipoRotaSemPararHelper());