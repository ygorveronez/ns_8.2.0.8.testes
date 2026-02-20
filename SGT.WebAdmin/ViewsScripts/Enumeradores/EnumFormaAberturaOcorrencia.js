var EnumFormaAberturaOcorrenciaHelper = function () {
    this.Todos = "";
    this.Email = 1;
    this.Portal = 2;
    this.Planilha = 3;
};

EnumFormaAberturaOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaAberturaOcorrencia.Email, value: this.Email },
            { text: Localization.Resources.Enumeradores.FormaAberturaOcorrencia.Portal, value: this.Portal },
            { text: Localization.Resources.Enumeradores.FormaAberturaOcorrencia.Planilha, value: this.Planilha }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.FormaAberturaOcorrencia.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaAberturaOcorrencia = Object.freeze(new EnumFormaAberturaOcorrenciaHelper());