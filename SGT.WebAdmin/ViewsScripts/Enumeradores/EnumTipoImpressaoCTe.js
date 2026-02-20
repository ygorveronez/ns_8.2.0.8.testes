var EnumTipoImpressaoCTeHelper = function () {
    this.Retrato = 1;
    this.Paisagem = 2;
};

EnumTipoImpressaoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoImpressaoCTe.Retrato, value: this.Retrato },
            { text: Localization.Resources.Enumeradores.TipoImpressaoCTe.Paisagem, value: this.Paisagem }
        ];
    },

};

var EnumTipoImpressaoCTe = Object.freeze(new EnumTipoImpressaoCTeHelper());