var EnumTipoTransportadorBiddingHelper = function () {
    this.Titular = 1;
    this.Spot = 2;
};

EnumTipoTransportadorBiddingHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Titular", value: this.Titular },
            { text: "Spot", value: this.Spot },
        ];
    }
};

var EnumTipoTransportadorBidding = Object.freeze(new EnumTipoTransportadorBiddingHelper());