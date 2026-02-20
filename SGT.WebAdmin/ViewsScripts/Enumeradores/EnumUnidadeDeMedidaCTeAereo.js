var EnumUnidadeDeMedidaCTeAereoHelper = function () {
    this.Todos = "";
    this.Quilograma = 1;
    this.QuilogramaBruto = 2;
    this.Litros = 3;
    this.TI = 4;
    this.Unidades = 5;
};

EnumUnidadeDeMedidaCTeAereoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.UnidadeDeMedidaCTeAereo.KG, value: this.Quilograma },
            { text: Localization.Resources.Enumeradores.UnidadeDeMedidaCTeAereo.KGBruto, value: this.QuilogramaBruto },
            { text: Localization.Resources.Enumeradores.UnidadeDeMedidaCTeAereo.Litros, value: this.Litros },
            { text: Localization.Resources.Enumeradores.UnidadeDeMedidaCTeAereo.TI, value: this.TI },
            { text: Localization.Resources.Enumeradores.UnidadeDeMedidaCTeAereo.Unidades, value: this.Unidades }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.UnidadeDeMedidaCTeAereo.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumUnidadeDeMedidaCTeAereo = Object.freeze(new EnumUnidadeDeMedidaCTeAereoHelper());