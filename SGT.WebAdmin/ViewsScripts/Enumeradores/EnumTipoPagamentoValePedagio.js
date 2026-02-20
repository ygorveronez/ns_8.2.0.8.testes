var EnumTipoPagamentoValePedagioHelper = function () {
    this.Cartao = 1;
    this.Tag = 2;
}

EnumTipoPagamentoValePedagioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPagamentoValePedagio.Cartao, value: this.Cartao },
            { text: Localization.Resources.Enumeradores.TipoPagamentoValePedagio.Tag, value: this.Tag },
        ];
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Cartao: return Localization.Resources.Enumeradores.TipoPagamentoValePedagio.Cartao;
            case this.Tag: return Localization.Resources.Enumeradores.TipoPagamentoValePedagio.Tag;
            default: return "";
        }
    }
};

var EnumTipoPagamentoValePedagio = Object.freeze(new EnumTipoPagamentoValePedagioHelper());