var EnumModalEnvioDocumentacaoHelper = function () {
    this.PortoDestino = 1;
    this.PortaDestino = 2;
};

EnumModalEnvioDocumentacaoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.PortoDestino: return "PortO no Destino";
            case this.PortaDestino: return "PortA no Destino";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "PortO no Destino", value: this.PortoDestino },
            { text: "PortA no Destino", value: this.PortaDestino }
        ];
    }
};

var EnumModalEnvioDocumentacao = Object.freeze(new EnumModalEnvioDocumentacaoHelper());