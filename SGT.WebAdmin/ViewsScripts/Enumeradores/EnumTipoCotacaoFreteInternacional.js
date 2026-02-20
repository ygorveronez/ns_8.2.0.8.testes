var EnumTipoCotacaoFreteInternacionalHelper = function () {
    this.CotacaoCorrente = 0;
    this.CotacaoMinima = 1;
    this.CotacaoFixa = 2;
};

EnumTipoCotacaoFreteInternacionalHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCotacaoFreteInternacional.CotacaoCorrente, value: this.CotacaoCorrente },
            { text: Localization.Resources.Enumeradores.TipoCotacaoFreteInternacional.CotacaoMinima, value: this.CotacaoMinima },
            { text: Localization.Resources.Enumeradores.TipoCotacaoFreteInternacional.CotacaoFixa, value: this.CotacaoFixa }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoCotacaoFreteInternacional.Todos, value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoCotacaoFreteInternacional = Object.freeze(new EnumTipoCotacaoFreteInternacionalHelper());