var EnumTipoTomaodorHelper = function () {
    this.Outros = 4;
    this.Destinatario = 3;
    this.Remetente = 0;
    this.MesmoDoCTe = 99;
};

EnumTipoTomaodorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTomaodor.MesmoDoCTe, value: this.MesmoDoCTe },
            { text: Localization.Resources.Enumeradores.TipoTomaodor.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TipoTomaodor.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.TipoTomaodor.Outros, value: this.Outros }
        ];
    },
};

var EnumTipoTomaodor = Object.freeze(new EnumTipoTomaodorHelper());
