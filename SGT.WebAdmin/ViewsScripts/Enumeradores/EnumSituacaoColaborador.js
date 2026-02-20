var EnumSituacaoColaboradorHelper = function () {
    this.Todos = "";
    this.Afastado = 1;
    this.Atestado = 2;
    this.Ferias = 3;
    this.Folga = 4;
    this.Suspenso = 5;
    this.Trabalhando = 6;
    this.DSR = 7;
    this.EmTreinamento = 8;
};

EnumSituacaoColaboradorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.Afastado, value: this.Afastado },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.Atestado, value: this.Atestado },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.Ferias, value: this.Ferias },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.Folga, value: this.Folga },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.Suspenso, value: this.Suspenso },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.Trabalhando, value: this.Trabalhando },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.DSR, value: this.DSR },
            { text: Localization.Resources.Enumeradores.SituacaoColaborador.EmTreinamento, value: this.EmTreinamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoColaborador.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoColaborador = Object.freeze(new EnumSituacaoColaboradorHelper());