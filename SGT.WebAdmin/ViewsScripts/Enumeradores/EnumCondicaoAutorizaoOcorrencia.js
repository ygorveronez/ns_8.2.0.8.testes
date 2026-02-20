var EnumCondicaoAutorizaoOcorrenciaHelper = function () {
    this.IgualA = 1;
    this.DiferenteDe = 2;
    this.MaiorQue = 3;
    this.MaiorIgualQue = 4;
    this.MenorQue = 5;
    this.MenorIgualQue = 6;
};

EnumCondicaoAutorizaoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA, value: this.IgualA },
            { text: Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe, value: this.DiferenteDe },
            { text: Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue, value: this.MaiorQue },
            { text: Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue, value: this.MaiorOuIgualQue },
            { text: Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue, value: this.MenorQue },
            { text: Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue, value: this.MenorOuIgualQue }
        ];
    },
    obterDescricao: function (enumerador) {
        switch (enumerador) {
            case EnumCondicaoAutorizaoOcorrencia.IgualA: return Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA;
            case EnumCondicaoAutorizaoOcorrencia.DiferenteDe: return Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe;
            case EnumCondicaoAutorizaoOcorrencia.MaiorIgualQue: return Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorOuIgualQue;
            case EnumCondicaoAutorizaoOcorrencia.MaiorQue: return Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue;
            case EnumCondicaoAutorizaoOcorrencia.MenorIgualQue: return Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue;
            case EnumCondicaoAutorizaoOcorrencia.MenorQue: return Localization.Resources.Enumeradores.CondicaoAutorizaoOcorrencia.MenorOuIgualQue;
            default: return "";
        }
    }
};

var EnumCondicaoAutorizaoOcorrencia = Object.freeze(new EnumCondicaoAutorizaoOcorrenciaHelper());