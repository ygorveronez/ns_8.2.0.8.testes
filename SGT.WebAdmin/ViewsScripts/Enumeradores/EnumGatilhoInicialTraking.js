var EnumGatilhoInicialTrakingHelper = function () {
    this.Todos = "";
    this.PrevisaoDescarga = 1;
    this.EntradaFronteira = 2;
    this.EntradaCliente = 3;
    this.InicioEntrega = 4;
    this.EntradaParqueamento = 5;
};

EnumGatilhoInicialTrakingHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.GatilhoInicialTraking.PrevisaoDescarga, value: this.PrevisaoDescarga },
            { text: Localization.Resources.Enumeradores.GatilhoInicialTraking.EntradaFronteira, value: this.EntradaFronteira },
            { text: Localization.Resources.Enumeradores.GatilhoInicialTraking.EntradaParqueamento, value: this.EntradaParqueamento },
            { text: Localization.Resources.Enumeradores.GatilhoInicialTraking.EntradaCliente, value: this.EntradaCliente },
            { text: Localization.Resources.Enumeradores.GatilhoInicialTraking.InicioEntrega, value: this.InicioEntrega },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.GatilhoInicialTraking.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumGatilhoInicialTraking = Object.freeze(new EnumGatilhoInicialTrakingHelper());