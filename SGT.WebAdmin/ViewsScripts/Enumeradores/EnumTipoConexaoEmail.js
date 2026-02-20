var EnumTipoConexaoEmailHelper = function () {
    this.Padrao = 0;
    this.Gmail = 1;
    this.Exchange = 2;
};

EnumTipoConexaoEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoConexaoEmail.Padrao, value: this.Padrao },
            { text: Localization.Resources.Enumeradores.TipoConexaoEmail.Gmail, value: this.Gmail },
            { text: Localization.Resources.Enumeradores.TipoConexaoEmail.Exchange, value: this.Exchange }
        ];
    }
};

var EnumTipoConexaoEmail = Object.freeze(new EnumTipoConexaoEmailHelper());