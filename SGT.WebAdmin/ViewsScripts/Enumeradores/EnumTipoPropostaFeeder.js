var EnumTipoPropostaFeederHelper = function () {
    this.Todos = "";
    this.Mercosul = 0;
    this.Alianca = 1;
    this.HSBR = 2;
    this.Outros = 3;
};

EnumTipoPropostaFeederHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Mercosul", value: this.Mercosul },
            { text: "Aliança", value: this.Alianca },
            { text: "HSBR", value: this.HSBR },
            { text: Localization.Resources.Enumeradores.TipoPropostaFeeder.Outros, value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPropostaFeeder.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPropostaFeeder = Object.freeze(new EnumTipoPropostaFeederHelper());