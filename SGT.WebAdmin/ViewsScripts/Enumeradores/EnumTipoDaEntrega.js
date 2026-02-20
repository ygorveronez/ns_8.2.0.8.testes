var EnumTipoDaEntregaHelper = function () {
    this.Nenhum = "";
    this.Entrega = 1;
    this.Coleta = 2;
};

EnumTipoDaEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDaEntrega.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoDaEntrega.Entrega, value: this.Entrega },
            { text: Localization.Resources.Enumeradores.TipoDaEntrega.Coleta, value: this.Coleta },
        ];
    },
};

var EnumTipoDaEntrega = Object.freeze(new EnumTipoDaEntregaHelper());