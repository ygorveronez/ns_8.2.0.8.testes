var EnumProtocoloHelper = function () {
    this.HTTP = 1;
    this.HTTPS = 2;
    this.TCP = 3;
};

EnumProtocoloHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "HTTP", value: this.HTTP },
            { text: "HTTPS", value: this.HTTPS },
            { text: "TCP", value: this.TCP }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumProtocolo = Object.freeze(new EnumProtocoloHelper());