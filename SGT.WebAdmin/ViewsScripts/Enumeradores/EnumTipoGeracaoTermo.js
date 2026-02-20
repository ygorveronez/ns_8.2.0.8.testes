var EnumTipoGeracaoTermoHelper = function () {
    this.Nenhum = "";
    this.Automatico = 1;
    this.Manual = 2;
};

EnumTipoGeracaoTermoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Transportadores.Transportador.Automatico, value: this.Automatico },
            { text: Localization.Resources.Transportadores.Transportador.Manual, value: this.Manual }
        ];
    },
    obterOpcoesTodos: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Nenhum, value: this.Nenhum }].concat(this.obterOpcoes());
    },
}

var EnumTipoGeracaoTermo = Object.freeze(new EnumTipoGeracaoTermoHelper());