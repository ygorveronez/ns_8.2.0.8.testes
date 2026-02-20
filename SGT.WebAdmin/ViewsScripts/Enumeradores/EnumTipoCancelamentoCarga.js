var EnumTipoCancelamentoCargaHelper = function () {
    this.Cancelamento = 0;
    this.Anulacao = 1;
};

EnumTipoCancelamentoCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Cancelada, value: this.Cancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Anulada, value: this.Anulacao },
        ];
    },

    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Todas, value: null }].concat(this.ObterOpcoes());
    },
};

var EnumTipoCancelamentoCarga = Object.freeze(new EnumTipoCancelamentoCargaHelper());