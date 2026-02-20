var EnumSituacaoDigitalizacaoCanhotoHelper = function () {
    this.Todas = 0;
    this.PendenteDigitalizacao = 1;
    this.AgAprovocao = 2;
    this.Digitalizado = 3;
    this.DigitalizacaoRejeitada = 4;
    this.Cancelada = 5;
    this.AgIntegracao = 6;
    this.ValidacaoEmbarcador = 7;
};

EnumSituacaoDigitalizacaoCanhotoHelper.prototype = {
    ObterOpcoes: function (exibirOpcaoCancelada) {
        var opcoes = [
            { text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.AguardandoAprovacao, value: this.AgAprovocao },
            { text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada, value: this.DigitalizacaoRejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado, value: this.Digitalizado },
            { text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.Pendente, value: this.PendenteDigitalizacao },
            { text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.AgIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.ValidacaoEmbarcador, value: this.ValidacaoEmbarcador }
        ];

        if (exibirOpcaoCancelada)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.Cancelada, value: this.Cancelada });

        return opcoes;
    },
    ObterOpcoesPesquisa: function (exibirOpcaoCancelada) {
        return [{ text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.Todas, value: this.Todas }].concat(this.ObterOpcoes(exibirOpcaoCancelada));
    },
    ObterOpcoesPesquisaComPlaceHolder: function (exibirOpcaoCancelada) {
        return [{ text: Localization.Resources.Enumeradores.SituacaoDigitalizacaoCanhoto.TodasSituacoesDigitalizacao, value: this.Todas }].concat(this.ObterOpcoes(exibirOpcaoCancelada));
    }
};

var EnumSituacaoDigitalizacaoCanhoto = Object.freeze(new EnumSituacaoDigitalizacaoCanhotoHelper());
