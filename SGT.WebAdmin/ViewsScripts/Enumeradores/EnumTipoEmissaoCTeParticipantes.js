var EnumTipoEmissaoCTeParticipantesHelper = function () {
    this.Normal = 0;
    this.ComRecebedor = 1;
    this.ComExpedidor = 2;
    this.ComTransbordo = 3;
    this.ComExpedidorERecebedor = 4;
};

EnumTipoEmissaoCTeParticipantesHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeParticipantes.NormalRemetenteDestinatario, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor, value: this.ComRecebedor },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor, value: this.ComExpedidor },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeParticipantes.ComTransbordo, value: this.ComTransbordo },
            { text: Localization.Resources.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorRecebedor, value: this.ComExpedidorERecebedor },
        ];
    }
};

var EnumTipoEmissaoCTeParticipantes = Object.freeze(new EnumTipoEmissaoCTeParticipantesHelper());