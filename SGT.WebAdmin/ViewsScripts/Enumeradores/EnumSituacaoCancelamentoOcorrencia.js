var EnumSituacaoCancelamentoOcorrenciaHelper = function () {
    this.Todas = "";
    this.EmCancelamento = 1;
    this.Cancelada = 2;
    this.RejeicaoCancelamento = 3;
    this.AguardandoIntegracao = 4;
    this.FalhaIntegracao = 5;
    this.Cancelamento = 6;
    this.Anulacao = 7;
};

EnumSituacaoCancelamentoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.AguardandoIntegracao, value: this.AguardandoIntegracao },
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.EmCancelamento, value: this.EmCancelamento },
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.FalhaIntegracao, value: this.FalhaIntegracao },
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.RejeicaoCancelamento, value: this.RejeicaoCancelamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.Todas ,value: this.Todas }].concat(this.obterOpcoes());
    },

    obterOpcoesCancelamentoOcorrencia: function () {
        return [
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.Cancelamento, value: this.Cancelamento },
            { text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.Anulacao, value: this.Anulacao }
        ];
    },
    obterOpcoeCancelamentoOcorrenciaPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.CancelamentoOcorrencia.Todas, value: this.Todas }].concat(this.obterOpcoesCancelamentoOcorrencia());
    }
}

var EnumSituacaoCancelamentoOcorrencia = Object.freeze(new EnumSituacaoCancelamentoOcorrenciaHelper());