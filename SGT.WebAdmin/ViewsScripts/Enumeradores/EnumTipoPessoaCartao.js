var EnumTipoPessoaCartaoHelper = function () {
    this.Nenhum = "";
    this.Fisica = 1;
    this.Juridica = 2;
};

EnumTipoPessoaCartaoHelper.prototype = {
    obterOpcoes: function () {
        var options = [
            { text: Localization.Resources.Gerais.Geral.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoPessoa.Fisica, value: this.Fisica },
            { text: Localization.Resources.Enumeradores.TipoPessoa.Juridica, value: this.Juridica }
        ];

        return options;
    },
};

var EnumTipoPessoaCartao = Object.freeze(new EnumTipoPessoaCartaoHelper());