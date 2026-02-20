var EnumTipoPerguntasHelper = function () {
    this.SemResposta = 0;
    this.Sim = 1;
    this.Nao = 2;

};

EnumTipoPerguntasHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPerguntas.SemResposta, value: this.SemResposta },
            { text: Localization.Resources.Enumeradores.TipoPerguntas.Sim, value: this.Sim },
            { text: Localization.Resources.Enumeradores.TipoPerguntas.Nao, value: this.Nao },
        ];
    },
    obterResposta: function (enumerador) {
        if (enumerador == this.Sim)
            return Localization.Resources.Enumeradores.TipoPerguntas.Sim;
        if (enumerador == this.Nao)
            return Localization.Resources.Enumeradores.TipoPerguntas.Nao;

        return "";
    }
};

var EnumTipoPerguntas = Object.freeze(new EnumTipoPerguntasHelper());