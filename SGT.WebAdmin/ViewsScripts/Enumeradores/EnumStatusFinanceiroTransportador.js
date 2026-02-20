var EnumStatusFinanceiroTransportadorlHelper = function () {
    this.Normal = "N";
    this.ComPendencias = "B";
};

EnumStatusFinanceiroTransportadorlHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusFinanceiroTransportador.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.StatusFinanceiroTransportador.ComPendencias, value: this.ComPendencias }
        ];
    }
};

var EnumStatusFinanceiroTransportador = Object.freeze(new EnumStatusFinanceiroTransportadorlHelper());