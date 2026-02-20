var EnumTipoObservacaoCTeHelper = function () {
    this.Contribuinte = 0;
    this.Fisco = 1;
};

EnumTipoObservacaoCTeHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoObservacaoCTe.Contribuinte, value: this.Contribuinte },
            { text: Localization.Resources.Enumeradores.TipoObservacaoCTe.Fisco, value: this.Fisco }
        ];
    },
    ObterDescricao: function (tipo) {
        switch (tipo) {
            case this.Contribuinte: return Localization.Resources.Enumeradores.TipoObservacaoCTe.Contribuinte;
            case this.Fisco: return Localization.Resources.Enumeradores.TipoObservacaoCTe.Fisco;
            default: return "";
        }
    }
};

var EnumTipoObservacaoCTe = Object.freeze(new EnumTipoObservacaoCTeHelper());