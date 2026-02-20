var EnumNetworkContratoTransporteHelper = function () {
    this.MarketAfrica = 0
    this.MarketAsia = 1;
    this.MarketEurope = 2;
    this.MarketNamet = 3;
    this.MarketNorthAmerica = 4;
    this.MarketSouthAmerica = 5;
};

EnumNetworkContratoTransporteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Market Africa", value: this.MarketAfrica },
            { text: "Market Asia", value: this.MarketAsia },
            { text: "Market Europe", value: this.MarketEurope },
            { text: "Market Namet", value: this.MarketNamet },
            { text: "Market North America", value: this.MarketNorthAmerica },
            { text: "Market South America", value: this.MarketSouthAmerica },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumNetworkContratoTransporte = Object.freeze(new EnumNetworkContratoTransporteHelper());