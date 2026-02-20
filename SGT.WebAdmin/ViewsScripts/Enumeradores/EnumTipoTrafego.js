var EnumTipoTrafegoHelper = function () {
    this.Todos = "";
    this.Proprio = 0;
    this.Mutuo = 1;
    this.Rodoferroviario = 2;
    this.Rodoviario = 3;
};

EnumTipoTrafegoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTrafego.Proprio, value: this.Proprio },
            { text: Localization.Resources.Enumeradores.TipoTrafego.Mutuo, value: this.Mutuo },
            { text: Localization.Resources.Enumeradores.TipoTrafego.Rodoferroviario, value: this.Rodoferroviario },
            { text: Localization.Resources.Enumeradores.TipoTrafego.Rodoviario, value: this.Rodoviario }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoTrafego.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTrafego = Object.freeze(new EnumTipoTrafegoHelper());