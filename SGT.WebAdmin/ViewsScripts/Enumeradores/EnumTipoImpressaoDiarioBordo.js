var EnumTipoImpressaoDiarioBordoHelper = function () {
    this.Nenhum = 0;
    this.DiarioBordo = 1;
    this.MinutaFreteBovino = 2;
};

EnumTipoImpressaoDiarioBordoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoImpressaoDiarioBordo.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoImpressaoDiarioBordo.DiarioBordo, value: this.DiarioBordo },
            { text: Localization.Resources.Enumeradores.TipoImpressaoDiarioBordo.MinutaFreteBovino, value: this.MinutaFreteBovino }
        ];
    }
};

var EnumTipoImpressaoDiarioBordo = Object.freeze(new EnumTipoImpressaoDiarioBordoHelper());