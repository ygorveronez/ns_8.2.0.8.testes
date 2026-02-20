var EnumEtapaAutorizacaoOcorrenciaHelper = function () {
    this.Todas = "";
    this.AprovacaoOcorrencia = 0;
    this.EmissaoOcorrencia = 1;
};

EnumEtapaAutorizacaoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia, value: this.AprovacaoOcorrencia },
            { text: Localization.Resources.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia, value: this.EmissaoOcorrencia }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.EtapaAutorizacaoOcorrencia.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesDuas: function () {
        return [
            { text: Localization.Resources.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia, value: this.AprovacaoOcorrencia },
            { text: Localization.Resources.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia, value: this.EmissaoOcorrencia }
        ];
    },
};

var EnumEtapaAutorizacaoOcorrencia = Object.freeze(new EnumEtapaAutorizacaoOcorrenciaHelper());