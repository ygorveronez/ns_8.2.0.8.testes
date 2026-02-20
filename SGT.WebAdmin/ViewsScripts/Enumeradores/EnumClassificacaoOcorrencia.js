var EnumClassificacaoOcorrenciaHelper = function () {
    this.Nenhum = "";
    this.DeslocamentoTraking = 1;
    this.FreteNegociado = 2;

};

EnumClassificacaoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ClassificacaoOcorrencia.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.ClassificacaoOcorrencia.DeslocamentoTraking, value: this.DeslocamentoTraking },
            { text: Localization.Resources.Enumeradores.ClassificacaoOcorrencia.FreteNegociado, value: this.FreteNegociado }
        ];
    },
}

var EnumClassificacaoOcorrencia = Object.freeze(new EnumClassificacaoOcorrenciaHelper());