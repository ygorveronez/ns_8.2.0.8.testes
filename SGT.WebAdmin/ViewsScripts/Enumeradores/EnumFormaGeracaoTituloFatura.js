var EnumFormaGeracaoTituloFaturaHelper = function () {    
    this.Todos = "";
    this.Padrao = 0;
    this.PorDocumento = 1;
    this.PorParcela = 2;
};

EnumFormaGeracaoTituloFaturaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaGeracaoTituloFatura.UsarPadrao, value: this.Padrao },
            { text: Localization.Resources.Enumeradores.FormaGeracaoTituloFatura.GerarTituloPorDocumento, value: this.PorDocumento },
            { text: Localization.Resources.Enumeradores.FormaGeracaoTituloFatura.GerarTituloPorParcela, value: this.PorParcela }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.FormaGeracaoTituloFatura.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaGeracaoTituloFatura = Object.freeze(new EnumFormaGeracaoTituloFaturaHelper());