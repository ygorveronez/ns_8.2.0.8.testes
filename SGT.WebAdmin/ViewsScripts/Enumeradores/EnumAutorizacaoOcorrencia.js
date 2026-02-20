var EnumAutorizacaoOcorrenciaHelper = function () {
    this.Todas = "";
    this.AprovacaoOcorrencia = 1;
    this.EmissaoOcorrencia = 2;
    this.ConfCTeAnterior = 3;
    this.Destinatario = 4;
    this.Remetente = 5;
};

EnumAutorizacaoOcorrenciaHelper.prototype = {
    obterOpcoesEtapaAutorizacaoOcorrencia: function () {
        return [
            { text: Localization.Resources.Enumeradores.AutorizacaoOcorrencia.AprovacaoOcorrencia, value: this.AprovacaoOcorrencia },
            { text: Localization.Resources.Enumeradores.AutorizacaoOcorrencia.EmissaoOcorrencia, value: this.EmissaoOcorrencia }
        ];
    },
    obterOpcoesPesquisaEtapaAutorizacaoOcorrencia: function () {
        return [{ text: Localization.Resources.Enumeradores.AutorizacaoOcorrencia.Todas, value: this.Todas }].concat(this.obterOpcoesEtapaAutorizacaoOcorrencia());
    },
    obterOpcoesResponsavelOcorrencia: function () {
        return [
            { text: Localization.Resources.Enumeradores.AutorizacaoOcorrencia.ConfCTeAnterior, value: this.ConfCTeAnterior },
            { text: Localization.Resources.Enumeradores.AutorizacaoOcorrencia.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.AutorizacaoOcorrencia.Remetente, value: this.Remetente }
        ];
    }
};

var EnumAutorizacaoOcorrencia = Object.freeze(new EnumAutorizacaoOcorrenciaHelper());