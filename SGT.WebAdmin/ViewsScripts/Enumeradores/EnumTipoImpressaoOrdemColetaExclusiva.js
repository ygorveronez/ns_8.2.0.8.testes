var EnumTipoImpressaoOrdemColetaExclusivaHelper = function () {
    this.Retrato = 0;
    this.Paisagem = 1;
};

EnumTipoImpressaoOrdemColetaExclusivaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoImpressaoOrdemColetaExclusiva.Retrato, value: this.Retrato },
            { text: Localization.Resources.Enumeradores.TipoImpressaoOrdemColetaExclusiva.Paisagem, value: this.Paisagem }
        ];
    }
};

var EnumTipoImpressaoOrdemColetaExclusiva = Object.freeze(new EnumTipoImpressaoOrdemColetaExclusivaHelper());