var EnumFormaEmissaoSVMHelper = function () {
    this.Nenhum = 0;
    this.PortoOrigem = 1;
    this.PortoTransbordo = 2;
    this.PortoDestino = 3;
}

EnumFormaEmissaoSVMHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return "Nenhum";
            case this.PortoOrigem: return "Porto de Origem";
            case this.PortoTransbordo: return "Porto de Transbordo";
            case this.PortoDestino: return "Porto de Destino";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "Porto de Origem", value: this.PortoOrigem },
            { text: "Porto de Transbordo", value: this.PortoTransbordo },
            { text: "Porto de Destino", value: this.PortoDestino }
        ];
    }
}

var EnumFormaEmissaoSVM = Object.freeze(new EnumFormaEmissaoSVMHelper());