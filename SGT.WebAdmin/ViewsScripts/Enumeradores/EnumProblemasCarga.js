var EnumProblemasCargaHelper = function () {
    this.Nenhum = 0;
    this.ProblemasCTe = 1;
    this.ProblemasAverbacao = 2;
    this.ProblemasValePedagio = 3;
    this.ProblemasMDFe = 4;
};

EnumProblemasCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ProblemasCarga.ProblemasCTe, value: this.ProblemasCTe },
            { text: Localization.Resources.Enumeradores.ProblemasCarga.ProblemasAverbacao, value: this.ProblemasAverbacao },
            { text: Localization.Resources.Enumeradores.ProblemasCarga.ProblemasValePedagio, value: this.ProblemasValePedagio },
            { text: Localization.Resources.Enumeradores.ProblemasCarga.ProblemasMDFe, value: this.ProblemasMDFe }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ProblemasCarga.Nenhum, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumProblemasCarga = Object.freeze(new EnumProblemasCargaHelper());