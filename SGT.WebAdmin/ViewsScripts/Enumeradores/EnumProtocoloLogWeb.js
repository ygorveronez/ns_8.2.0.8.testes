var EnumProtocoloLogWebHelper = function () {
    this.UDP = 0;
    this.TCP = 1;
};

EnumProtocoloLogWebHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "UDP", value: this.UDP },
            { text: "TCP", value: this.TCP }
        ];
    }
};

var EnumProtocoloLogWeb = Object.freeze(new EnumProtocoloLogWebHelper());
