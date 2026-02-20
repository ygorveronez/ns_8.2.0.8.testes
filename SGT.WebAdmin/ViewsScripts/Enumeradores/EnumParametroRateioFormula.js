var EnumParametroRateioFormulaHelper = function () {
    this.Todos = 0;
    this.Peso = 1;
    this.Distancia = 2;
    this.PorNotaFiscal = 3;
    this.ValorMercadoria = 4;
    this.PorCTe = 5;
    this.MetroCubico = 7;
    this.PorFracionadaTonelada = 8;
    this.PorFracionadaMetroCubico = 9;
    this.PesoLiquido = 10.;
    this.Volume = 11;
    this.FatorPonderacaoDistanciaPeso = 12;
    this.PorPesoUtilizandoFatorCubagem = 13;
};

EnumParametroRateioFormulaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por peso", value: this.Peso },
            { text: "Por nota fiscal", value: this.PorNotaFiscal },
            { text: "Por CT-e", value: this.PorCTe },
            { text: "Por valor da mercadoria", value: this.ValorMercadoria },
            { text: "Por metros cúbicos", value: this.MetroCubico },
            { text: "Por fracionada em tonelada", value: this.PorFracionadaTonelada },
            { text: "Por fracionada em metros cúbicos", value: this.PorFracionadaMetroCubico },
            { text: "Por peso líquido", value: this.PesoLiquido },
            { text: "Por volume", value: this.Volume },
            { text: "Por fator de ponderação entre distância e peso", value: this.FatorPonderacaoDistanciaPeso },
            { text: "Por peso utilizando fator de cubagem", value: this.PorPesoUtilizandoFatorCubagem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesOcorrencia: function () {
        return [
            { text: "Padrão", value: this.Todos },
            { text: "Por peso", value: this.Peso },
            { text: "Por valor da mercadoria", value: this.ValorMercadoria },
            { text: "Por CT-e", value: this.PorCTe },
        ];
    }
};

var EnumParametroRateioFormula = Object.freeze(new EnumParametroRateioFormulaHelper());