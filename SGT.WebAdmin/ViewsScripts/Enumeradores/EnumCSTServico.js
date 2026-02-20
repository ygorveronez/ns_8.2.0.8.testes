var EnumCSTServicoHelper = function () {
    this.Todas = "";
    this.TributadaIntegralmente = 0;
    this.TributadaIntegralmenteISSRF = 1;
    this.TributadaIntegralmenteST = 2;
    this.TributadaReducaoBC = 3;
    this.TributadaReducaoBCISSRF = 4;
    this.TributadaReducaoBCST = 5;
    this.Isenta = 6;
    this.Imune = 7;
    this.NaoTributadaISSRegimeFixo = 8;
    this.NaoTributadaISSRegimeEstimativa = 9;
    this.NaoTributadaISSConstrucaoCivil = 10;
    this.NaoTributadaISSRecolhidoNotaAvulsa = 11;
    this.NaoTributadaPrestadorEstabelecidoMunicipio = 12;
    this.NaoTributadaRecolhimentoForaMunicipio = 13;
    this.NaoTributada = 14;
    this.NaoTributadaAtoCooperado = 15;
    this.ProdutosDocumentoFiscalConjugado = 99;
};

EnumCSTServicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "00 - Tributada Integralmente", value: this.TributadaIntegralmente },
            { text: "01 - Tributada Integralmente com ISSRF", value: this.TributadaIntegralmenteISSRF },
            { text: "02 - Tributada Integralmente e sujeita à Substituição Tributária", value: this.TributadaIntegralmenteST },
            { text: "03 - Tributada com redução da base de cálculo", value: this.TributadaReducaoBC },
            { text: "04 - Tributada com redução da base de cálculo com ISSRF", value: this.TributadaReducaoBCISSRF },
            { text: "05 - Tributada com redução da base de cálculo e sujeita à Substituição Tributária", value: this.TributadaReducaoBCST },
            { text: "06 - Isenta", value: this.Isenta },
            { text: "07 - Imune", value: this.Imune },
            { text: "08 - Não Tributada - ISS regime Fixo", value: this.NaoTributadaISSRegimeFixo },
            { text: "09 - Não Tributada - ISS regime Estimativa", value: this.NaoTributadaISSRegimeEstimativa },
            { text: "10 - Não Tributada - ISS Construção Civil recolhido antecipadamente", value: this.NaoTributadaISSConstrucaoCivil },
            { text: "11 - Não Tributada - ISS recolhido por Nota Avulsa", value: this.NaoTributadaISSRecolhidoNotaAvulsa },
            { text: "12 - Não Tributada - Prestador estabelecido no Município", value: this.NaoTributadaPrestadorEstabelecidoMunicipio },
            { text: "13 - Não Tributada - Recolhimento efetuado pelo prestador de fora do Município", value: this.NaoTributadaRecolhimentoForaMunicipio },
            { text: "14 - Não Tributada", value: this.NaoTributada },
            { text: "15 - Não Tributada - Ato Cooperado", value: this.NaoTributadaAtoCooperado },
            { text: "99 - Produtos Documento Fiscal Conjugado", value: this.ProdutosDocumentoFiscalConjugado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Nenhum", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumCSTServico = Object.freeze(new EnumCSTServicoHelper());