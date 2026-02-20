var EnumSituacaoOcorrenciaHelper = function () {
    this.Todas = 0;
    this.AgConfirmacaoUso = 1;
    this.AgAprovacao = 2;
    this.Finalizada = 3;
    this.Rejeitada = 4;
    this.RejeitadaEtapaEmissao = 19;
    this.AgEmissaoCTeComplementar = 6;
    this.EmEmissaoCTeComplementar = 7;
    this.EmCancelamento = 8;
    this.Cancelada = 9;
    this.PendenciaEmissao = 10;
    this.RejeicaoCancelamento = 11;
    this.AgIntegracao = 12;
    this.FalhaIntegracao = 13;
    this.AgInformacoes = 14;
    this.AgAutorizacaoEmissao = 15;
    this.AutorizacaoPendente = 16;
    this.SemRegraAprovacao = 17;
    this.SemRegraEmissao = 18;
    this.AgAceiteTransportador = 20;
    this.DebitoRejeitadoTransportador = 21;
    this.Anulada = 22;
};

EnumSituacaoOcorrenciaHelper.prototype = {
    isPermiteVoltarParaEtapaCadastro: function (situacao) {
        return (
            (situacao == this.AgAprovacao) ||
            (situacao == this.Rejeitada) ||
            (situacao == this.SemRegraAprovacao)
        );
    },
    obterOpcoes: function () {
        var situacoes = [];

        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAceiteTransportador, value: this.AgAceiteTransportador });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacao, value: this.AgAprovacao });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacaoEmissao, value: this.AgAutorizacaoEmissao });

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgInformacoes, value: this.AgInformacoes });

        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgIntegracao, value: this.AgIntegracao });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Cancelada, value: this.Cancelada });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.EmEmissaoDocumentoComplementar, value: this.EmEmissaoCTeComplementar });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Finalizada, value: this.Finalizada });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.ProblemaEmissao, value: this.PendenciaEmissao });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.ProblemaIntegracao, value: this.FalhaIntegracao });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Rejeitada, value: this.Rejeitada });
        situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao, value: this.RejeitadaEtapaEmissao });

        if (_CONFIGURACAO_TMS.ObrigatorioRegrasOcorrencia) {
            situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.SemRegraAprovacao, value: this.SemRegraAprovacao });
            situacoes.push({ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.SemRegraEmissao, value: this.SemRegraEmissao });
        }

        return situacoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaAutorizacao: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacao, value: this.AgAprovacao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacaoEmissao, value: this.AgAutorizacaoEmissao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgInformacoes, value: this.AgInformacoes },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AprovacaoPendente, value: this.AutorizacaoPendente },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Finalizada, value: this.Finalizada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Rejeitada, value: this.Rejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao, value: this.RejeitadaEtapaEmissao }
        ];
    },
    obterOpcoesPesquisaRelatorio: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAceiteTransportador, value: this.AgAceiteTransportador },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacao, value: this.AgAprovacao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacaoEmissao, value: this.AgAutorizacaoEmissao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgInformacoes, value: this.AgInformacoes },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Anulada, value: this.Anulada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.EmCancelamento, value: this.EmCancelamento },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Finalizada, value: this.Finalizada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.PendenciaEmissao, value: this.PendenciaEmissao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.ProblemaIntegracao, value: this.FalhaIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Rejeitada, value: this.Rejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao, value: this.RejeitadaEtapaEmissao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.RejeitadaCancelamento, value: this.RejeicaoCancelamento }
        ];
    },
    obterOpcoesPesquisaRelatorioCancelamento: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacao, value: this.AgAprovacao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacaoEmissao, value: this.AgAutorizacaoEmissao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgInformacoes, value: this.AgInformacoes },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.AgIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Anulada, value: this.Anulada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Finalizada, value: this.Finalizada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.PendenciaEmissao, value: this.PendenciaEmissao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.ProblemaIntegracao, value: this.FalhaIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.Rejeitada, value: this.Rejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao, value: this.RejeitadaEtapaEmissao }
        ];
    }
};

var EnumSituacaoOcorrencia = Object.freeze(new EnumSituacaoOcorrenciaHelper());
