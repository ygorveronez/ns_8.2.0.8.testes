var EnumTipoTermoHelper = function () {
    this.Nenhum = "";
    this.Unilateral = 1;
    this.Bilateral = 2;
};

EnumTipoTermoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Transportadores.Transportador.Unilateral, value: this.Unilateral },
            { text: Localization.Resources.Transportadores.Transportador.Bilateral, value: this.Bilateral }
        ];
    },
    obterOpcoesTodos: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Nenhum, value: this.Nenhum }].concat(this.obterOpcoes());
    },
}

var EnumTipoTermo = Object.freeze(new EnumTipoTermoHelper());