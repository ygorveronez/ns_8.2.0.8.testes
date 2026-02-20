var EnumSituacaoCancelamentoCTeSemCargaHelper = function () {
    this.Todos =0;
    this.AgCancelamentoCTe = 1,
    this.AgCancelamentoIntegracao = 2,
    this.Cancelado = 3,
    this.RejeicaoCancelamento = 4
};

EnumSituacaoCancelamentoCTeSemCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoCTe, value: this.AgCancelamentoCTe },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, value: this.RejeicaoCancelamento }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Todas, value: this.Todas }].concat(this.ObterOpcoes());
    },

    ObterOpcoesCancelamentoCTeSemCarga: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoCTe, value: this.AgCancelamentoCTe }
        ];
    },
    ObterOpcoesPesquisaCancelamentoCTeSemCarga: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Todas, value: this.Todas }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoCancelamentoCTeSemCarga = Object.freeze(new EnumSituacaoCancelamentoCTeSemCargaHelper());

