var EnumSituacaoCancelamentoCargaHelper = function () {
    this.Todas = "";
    this.AgConfirmacao = 0;
    this.EmCancelamento = 1;
    this.Cancelada = 2;
    this.RejeicaoCancelamento = 3;
    this.Anulada = 4;
    this.Reprovada = 5;
    this.AgCancelamentoMDFe = 6;
    this.AgCancelamentoAverbacaoMDFe = 7;
    this.AgCancelamentoCTe = 8;
    this.AgCancelamentoAverbacaoCTe = 9;
    this.AgIntegracao = 10;
    this.FinalizandoCancelamento = 11;
    this.AgAprovacaoSolicitacao = 12;
    this.SolicitacaoReprovada = 13;
    this.Cancelamento = 14;
    this.Anulacao = 15;
    this.AgIntegracaoDadosCancelamento = 16;
    this.AgIntegracaoCancelamentoCIOT = 17;
    this.AgCancelamentoFatura = 18;
};

EnumSituacaoCancelamentoCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgAprovacaoSolicitacao, value: this.AgAprovacaoSolicitacao },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe, value: this.AgCancelamentoAverbacaoCTe },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe, value: this.AgCancelamentoAverbacaoMDFe },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoCTe, value: this.AgCancelamentoCTe },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe, value: this.AgCancelamentoMDFe },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgConfirmacao, value: this.AgConfirmacao },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Anulada, value: this.Anulada },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento, value: this.EmCancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.FinalizandoCancelamento, value: this.FinalizandoCancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, value: this.RejeicaoCancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Reprovada, value: this.Reprovada },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.SolicitacaoReprovada, value: this.SolicitacaoReprovada },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoDadosCancelamento, value: this.AgIntegracaoDadosCancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT, value: this.AgIntegracaoCancelamentoCIOT },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoFatura, value: this.AgCancelamentoFatura }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Todas, value: this.Todas }].concat(this.ObterOpcoes());
    },

    ObterOpcoesCancelamentoCarga: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Cancelamento, value: this.Cancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Anulacao, value: this.Anulacao }
        ];
    },
    ObterOpcoesPesquisaCancelamentoCarga: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCancelamentoCarga.Todas, value: this.Todas }].concat(this.ObterOpcoesCancelamentoCarga());
    }
};

var EnumSituacaoCancelamentoCarga = Object.freeze(new EnumSituacaoCancelamentoCargaHelper());

