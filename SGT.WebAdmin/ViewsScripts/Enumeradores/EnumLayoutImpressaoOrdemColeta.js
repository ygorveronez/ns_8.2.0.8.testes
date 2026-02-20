var EnumLayoutImpressaoOrdemColetaHelper = function () {
    this.LayoutPadrao = 1;
    this.LayoutOrdemCarregamento = 2;
    this.LayoutColetaContainer = 3;
    this.LayoutOrdemColetaAuxiliar = 4;
};

EnumLayoutImpressaoOrdemColetaHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.LayoutPadrao: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutPadrao;
            case this.LayoutOrdemCarregamento: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento;
            case this.LayoutColetaContainer: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutColetaContainer;
            case this.LayoutOrdemColetaAuxiliar: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutPadrao, value: this.LayoutPadrao },
            { text: Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento, value: this.LayoutOrdemCarregamento },
            { text: Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutColetaContainer, value: this.LayoutColetaContainer },
            { text: Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar, value: this.LayoutOrdemColetaAuxiliar }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumLayoutImpressaoOrdemColeta = Object.freeze(new EnumLayoutImpressaoOrdemColetaHelper());