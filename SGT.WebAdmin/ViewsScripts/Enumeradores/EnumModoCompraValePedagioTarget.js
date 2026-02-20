var EnumModoCompraValePedagioTargetHelper = function () {
    this.CartaoTarget = 1;
    this.PedagioTagViaFacil = 2;
    this.PedagioTagVeloe = 5;
    this.PedagioTagMoveMais = 6;
    this.PedagioConectCar = 7;
    this.PedagioTagAmbipar = 8;
    this.PedagioTagRepom = 9;
    this.PedagioTagTicketLog = 10;
    this.PedagioTagEdenred = 11;
    this.PedagioTagAutomatico = 13,
    this.PedagioTagIndefinido = 14
};

EnumModoCompraValePedagioTargetHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Cartão Target", value: this.CartaoTarget },
            { text: "Tag Via Fácil (Sem Parar)", value: this.PedagioTagViaFacil },
            { text: "Tag Veloe", value: this.PedagioTagVeloe },
            { text: "Tag Move mais", value: this.PedagioTagMoveMais },
            { text: "Tag Conect Car", value: this.PedagioConectCar }
        ];
    },
    ObterOpcoesTag: function (TipoIntegracaoValePedagio) {
        let retorno = [
            { text: "Tag Via Fácil (Sem Parar)", value: this.PedagioTagViaFacil }
        ];

        if (TipoIntegracaoValePedagio != undefined && TipoIntegracaoValePedagio.length > 0) {
            if (TipoIntegracaoValePedagio.some(function (o) { return o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.EFrete || o == EnumTipoIntegracao.Pamcard || o == EnumTipoIntegracao.NDDCargo; })) {
                retorno = retorno.concat([{ text: "Tag Veloe", value: this.PedagioTagVeloe }]);
            }

            if (TipoIntegracaoValePedagio.some(function (o) { return o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.EFrete || o == EnumTipoIntegracao.Pamcard; })) {
                retorno = retorno.concat([{ text: "Tag Move mais", value: this.PedagioTagMoveMais }]);
            }

            if (TipoIntegracaoValePedagio.some(function (o) { return o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.EFrete || o == EnumTipoIntegracao.NDDCargo || o == EnumTipoIntegracao.Pamcard; })) {
                retorno = retorno.concat([{ text: "Tag Conect Car", value: this.PedagioConectCar }]);
            }

            if (TipoIntegracaoValePedagio.some(function (o) { return o == EnumTipoIntegracao.Ambipar || o == EnumTipoIntegracao.EFrete; })) {
                retorno = retorno.concat([{ text: "Tag Ambipar", value: this.PedagioTagAmbipar }]);
            }

            if (TipoIntegracaoValePedagio.some(function (o) { return o == EnumTipoIntegracao.EFrete; })) {
                retorno = retorno.concat([{ text: "Tag Repom", value: this.PedagioTagRepom }]);
                retorno = retorno.concat([{ text: "Tag Ticket Log", value: this.PedagioTagTicketLog }]);
                retorno = retorno.concat([{ text: "Tag Edenred", value: this.PedagioTagEdenred }]);
                retorno = retorno.concat([{ text: "Tag Automático", value: this.PedagioTagAutomatico }]);
                retorno = retorno.concat([{ text: "Tag Indefinido", value: this.PedagioTagIndefinido }]);
            }
        }

        return retorno;
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumModoCompraValePedagioTarget = Object.freeze(new EnumModoCompraValePedagioTargetHelper());