var EnumTipoPessoaHelper = function () {
    this.Todas = 99;
    this.Fisica = 0;
    this.Juridica = 1;
    this.Exterior = 2;
};

EnumTipoPessoaHelper.prototype = {
    obterOpcoes: function (comExterior = true) {
        var options = [
            { text: Localization.Resources.Enumeradores.TipoPessoa.Fisica, value: this.Fisica },
            { text: Localization.Resources.Enumeradores.TipoPessoa.Juridica, value: this.Juridica }
        ];

        if (comExterior)
            options.push({ text: Localization.Resources.Enumeradores.TipoPessoa.Exterior, value: this.Exterior });

        return options;
    },
    obterOpcoesPesquisa: function (comExterior = true) {
        return [{ text: Localization.Resources.Gerais.Geral.Todas, value: this.Todas }].concat(this.obterOpcoes(comExterior));
    },
    obterOpcoesPesquisaTexto: function (comExterior = true) {
        return [{ text: Localization.Resources.Gerais.Geral.Todas, value: "" },
                { text: Localization.Resources.Enumeradores.TipoPessoa.Fisica, value: "F" },
                { text: Localization.Resources.Enumeradores.TipoPessoa.Juridica, value: "J" },
                { text: Localization.Resources.Enumeradores.TipoPessoa.Exterior, value: "E" }];
    }
};

var EnumTipoPessoa = Object.freeze(new EnumTipoPessoaHelper());