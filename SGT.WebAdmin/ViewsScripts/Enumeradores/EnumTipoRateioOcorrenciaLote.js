var EnumTipoRateioOcorrenciaLoteHelper = function () {
    this.Todos = "";
    this.Peso = 1;
    this.ValorMercadoria = 2;
    this.QuantidadeCTe = 3;
};

EnumTipoRateioOcorrenciaLoteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por Peso", value: this.Peso },
            { text: "Por Valor da Mercadoria", value: this.ValorMercadoria },
            { text: "Por Quantidade de CT-e Emitido", value: this.QuantidadeCTe }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRateioOcorrenciaLote = Object.freeze(new EnumTipoRateioOcorrenciaLoteHelper());