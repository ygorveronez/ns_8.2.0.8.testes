var EnumTipoEnvioHUBOfertasHelper = function () {
    this.Todos = "";
    this.EnvioTransportador = 0;
    this.EnvioDemandaOferta = 1;
    this.CancelamentoDemandaOferta = 2;
    this.FinalizacaoDemandaOferta = 3;
};

EnumTipoEnvioHUBOfertasHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Envio Transportador", value: this.EnvioTransportador },
            { text: "Envio Demanda Oferta", value: this.EnvioDemandaOferta },
            { text: "Cancelamento Demanda Oferta", value: this.CancelamentoDemandaOferta },
            { text: "Finalização Demanda Oferta", value: this.FinalizacaoDemandaOferta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function (valor) {
        switch (valor) {
            case this.EnvioTransportador: return "Envio Transportador";
            case this.EnvioDemandaOferta: return "Envio Demanda Oferta";
            case this.CancelamentoDemandaOferta: return "Cancelamento Demanda Oferta";
            case this.FinalizacaoDemandaOferta: return "Finalização Demanda Oferta";
            default: return "";
        }
    }
};

var EnumTipoEnvioHUBOfertas = Object.freeze(new EnumTipoEnvioHUBOfertasHelper());