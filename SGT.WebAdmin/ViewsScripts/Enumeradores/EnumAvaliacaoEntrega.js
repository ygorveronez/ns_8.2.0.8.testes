var EnumRespondidaHelper = function () {
    this.Todos = null;
    this.NaoRepondida = 0;
    this.Respondida = 1;
};

EnumRespondidaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.AvaliacaoSituacao.Respondida, value: this.Respondida },
            { text: Localization.Resources.Enumeradores.AvaliacaoSituacao.NaoRespondida, value: this.NaoRepondida },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumRespondida = Object.freeze(new EnumRespondidaHelper());
