const EnumComportamentoHelper = function () {
    this.Todos = 0;
    this.Docil = 1;
    this.Obediente = 2;
    this.Agressivo = 3;
    this.Inquieto = 4;
    this.Medroso = 5;
};

EnumComportamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Comportamento.Docil, value: this.Docil },
            { text: Localization.Resources.Enumeradores.Comportamento.Obediente, value: this.Obediente },
            { text: Localization.Resources.Enumeradores.Comportamento.Agressivo, value: this.Agressivo },
            { text: Localization.Resources.Enumeradores.Comportamento.Inquieto, value: this.Inquieto },
            { text: Localization.Resources.Enumeradores.Comportamento.Medroso, value: this.Medroso }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Comportamento.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

const EnumComportamento = Object.freeze(new EnumComportamentoHelper());
