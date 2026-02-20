var EnumTipoBuscarPracasNaGeracaoDaCargaHelper = function () {
    this.Todos = "";
    this.OrigemDestino = 1;
    this.Polilinhas = 2;   
};

EnumTipoBuscarPracasNaGeracaoDaCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Origem e Destino", value: this.OrigemDestino },
            { text: "Polilinhas", value: this.Polilinhas }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoBuscarPracasNaGeracaoDaCarga = Object.freeze(new EnumTipoBuscarPracasNaGeracaoDaCargaHelper());