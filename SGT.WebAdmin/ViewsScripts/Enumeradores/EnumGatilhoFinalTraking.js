var EnumGatilhoFinalTrakingHelper = function () {
    this.Todos = "";
    this.SaidaCliente = 1;
    this.SaidaFronteira = 2;
    this.FimEntrega = 3;
    this.SaidaParqueamento = 4;
};

EnumGatilhoFinalTrakingHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.GatilhoFinalTraking.SaidaCliente, value: this.SaidaCliente },
            { text: Localization.Resources.Enumeradores.GatilhoFinalTraking.SaidaFronteira, value: this.SaidaFronteira },
            { text: Localization.Resources.Enumeradores.GatilhoFinalTraking.SaidaParqueamento, value: this.SaidaParqueamento },
            { text: Localization.Resources.Enumeradores.GatilhoFinalTraking.FimEntrega, value: this.FimEntrega },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.GatilhoFinalTraking.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumGatilhoFinalTraking = Object.freeze(new EnumGatilhoFinalTrakingHelper());