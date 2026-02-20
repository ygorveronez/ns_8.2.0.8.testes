var EnumRegraOfertaCargaHelper = function () {
    this.ValorFrete = 0;
    this.Share = 1;
    this.NivelServico = 2;
    
};


EnumRegraOfertaCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Valor de frete", value: this.ValorFrete },
            { text: "Share", value: this.Share },
            { text: "Nível de serviço", value: this.NivelServico }
        ];
    },
    obterTexto: function (valor) {
        switch (valor) {
            case this.ValorFrete:
                return "Valor de frete";
                break;
            case this.Share:
                return "Share";
                break;
            case this.NivelServico:
                return "Nível de serviço";
                break;
        }
    }
};


var EnumRegraOfertaCarga = Object.freeze(new EnumRegraOfertaCargaHelper());