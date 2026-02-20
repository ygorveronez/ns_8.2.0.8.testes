var EnumTipoProtocoloHelper = function () {
    this.Padrao = 0;
    this.HTTP = 1;
    this.HTTPS = 2;
};

EnumTipoProtocoloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "HTTP", value: this.HTTP },
            { text: "HTTPS", value: this.HTTPS }
        ];
    },
}

var EnumTipoProtocolo = Object.freeze(new EnumTipoProtocoloHelper());