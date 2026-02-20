var EnumTipoEventoHelper = function () {
    this.Almoco = 16;
    this.Espera = 17;
    this.Repouso = 18;
    this.Abastecimento = 19;
    this.Pernoite = 13;
};

EnumTipoEventoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEvento.Almoco, value: this.Almoco },
            { text: Localization.Resources.Enumeradores.TipoEvento.Espera, value: this.Espera },
            { text: Localization.Resources.Enumeradores.TipoEvento.Repouso, value: this.Repouso },
            { text: Localization.Resources.Enumeradores.TipoEvento.Abastecimento, value: this.Abastecimento },
            { text: Localization.Resources.Enumeradores.TipoEvento.Pernoite, value: this.Pernoite },
        ];
    }
};

var EnumTipoEvento = Object.freeze(new EnumTipoEventoHelper());