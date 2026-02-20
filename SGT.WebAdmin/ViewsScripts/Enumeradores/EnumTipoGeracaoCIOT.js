var EnumTipoGeracaoCIOTHelper = function () {
    this.PorViagem = 0;
    this.PorPeriodo = 1;
};

EnumTipoGeracaoCIOTHelper.prototype = {
    ObterDescricao: function (tipoGeracaoCIOT) {
        switch (tipoGeracaoCIOT) {
            case this.PorPeriodo: return Localization.Resources.Enumeradores.TipoGeracaoCIOT.PorPeriodo;
            case this.PorViagem: return Localization.Resources.Enumeradores.TipoGeracaoCIOT.PorViagem;
            default: return "";
        }
    },
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoGeracaoCIOT.UtilizarPadrao, value: "" },
            { text: Localization.Resources.Enumeradores.TipoGeracaoCIOT.PorViagem, value: this.PorViagem },
            { text: Localization.Resources.Enumeradores.TipoGeracaoCIOT.PorPeriodo, value: this.PorPeriodo }
        ];
    }
};

var EnumTipoGeracaoCIOT = Object.freeze(new EnumTipoGeracaoCIOTHelper());