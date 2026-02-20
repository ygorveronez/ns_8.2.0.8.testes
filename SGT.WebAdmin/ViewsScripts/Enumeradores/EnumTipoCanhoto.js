var EnumTipoCanhotoHelper = function () {
    this.Todos = 0;
    this.NFe = 1;
    this.Avulso = 2;
    this.CTeSubcontratacao = 3;
    this.CTe = 4;
};

EnumTipoCanhotoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCanhoto.NFe, value: this.NFe },
            { text: Localization.Resources.Enumeradores.TipoCanhoto.Avulso, value: this.Avulso },
            { text: Localization.Resources.Enumeradores.TipoCanhoto.CTeParaSubcontratacao, value: this.CTeSubcontratacao },
            { text: Localization.Resources.Enumeradores.TipoCanhoto.CTe, value: this.CTe }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoCanhoto.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaComPlaceHolder: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoCanhoto.TodosOsTiposDeCanhoto, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCanhoto = Object.freeze(new EnumTipoCanhotoHelper());