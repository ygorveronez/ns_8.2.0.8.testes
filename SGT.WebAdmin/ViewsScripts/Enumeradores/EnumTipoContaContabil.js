var EnumTipoContaContabilHelper = function () {
    this.Todos = 0;
    this.FreteLiquido = 1;
    this.TotalReceber = 2;
    this.ICMS = 3;
    this.ICMSST = 4;
    this.PIS = 5;
    this.COFINS = 6;
    this.FreteLiquido9 = 9;
    this.FreteLiquido2 = 10;
    this.ISS = 11;
    this.ISSRetido = 12;
    this.FreteValor = 13;
    this.AdValorem = 14;
    this.GRIS = 15;
    this.Pedagio = 16;
    this.TaxaDescarga = 17;
    this.TaxaEntrega = 18;
    this.ImpostoValorAgregado = 19;
    this.CustoFixo = 20;
    this.FreteCaixa = 21;
    this.FreteKM = 22;
    this.FretePeso = 23;
    this.FreteViagem = 24;
    this.TaxaTotal = 25;
    this.Pernoite = 26;
    this.TotalReceberSemISS = 27;
    this.FreteLiquidoSemComponentesFrete = 28;
    this.Criacao = 30;
    this.FreteLiquidoTotal = 31;
    this.Estadia = 32;
    this.DevolucaoPorPeso = 33;
    this.DevolucaoPercentual = 34;
    this.Reentrega = 35;
    this.Infrutifera = 36;
    this.FreetimeCavaloAtrelado = 37;
    this.FreetimeCavaloDesatrelado = 38;
    this.FreetimePorto = 39;
    this.FreetimeCarreta = 40;
    this.Demurrage = 41;
    this.PagamentoDesova = 42;
    this.ViagemFrustradaNoDestino = 43;
    this.PacoteDeVistoriaSuframa = 44;
    this.Recusa = 45;
    this.ValorManual = 46;
    this.CBS = 47;
    this.IBSMunicipal = 48;
    this.IBSEstadual = 49;
};

EnumTipoContaContabilHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.FreteLiquido: return "Frete Líquido 01 (sem ICMS, ISS, PIS e COFINS)";
            case this.FreteLiquido2: return "Frete Líquido 02 (sem ICMS, ISS, PIS e COFINS)";
            case this.FreteLiquido9: return "Frete Líquido 09 (sem ICMS, ISS, PIS e COFINS)";
            case this.FreteLiquido9: return "Frete Líquido 09 (sem ICMS, ISS, PIS e COFINS)";
            case this.FreteLiquidoSemComponentesFrete: return "Frete Líquido sem Componentes de Frete";
            case this.TotalReceber: return "Total a Receber";
            case this.ICMS: return "ICMS";
            case this.FreteValor: return "Frete Valor (sem ICMS e ISS)";
            case this.ICMSST: return "ICMS ST";
            case this.PIS: return "PIS";
            case this.COFINS: return "COFINS";
            case this.ISS: return "ISS";
            case this.ISSRetido: return "ISS RETIDO";
            case this.AdValorem: return "Ad Valorem";
            case this.GRIS: return "GRIS";
            case this.Pedagio: return "Pedágio";
            case this.TaxaDescarga: return "Taxa de Descarga";
            case this.TaxaEntrega: return "Taxa de Entrega";
            case this.ImpostoValorAgregado: return "Imposto sobre Valor Agregado (IVA)";
            case this.CustoFixo: return "Custo Fixo";
            case this.FreteCaixa: return "Frete Caixa";
            case this.FreteKM: return "Frete KM";
            case this.FretePeso: return "Frete Peso";
            case this.FreteViagem: return "Frete Viagem";
            case this.TaxaTotal: return "Taxa Total";
            case this.Pernoite: return "Pernoite";
            case this.TotalReceberSemISS: return "Total a Receber sem ISS";
            case this.CBS: return "CBS";
            case this.IBSMunicipal: return "IBS Municipal";
            case this.IBSEstadual: return "IBS Estadual";

            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.FreteLiquido), value: this.FreteLiquido },
            { text: this.obterDescricao(this.FreteLiquido2), value: this.FreteLiquido2 },
            { text: this.obterDescricao(this.FreteLiquido9), value: this.FreteLiquido9 },
            { text: this.obterDescricao(this.FreteLiquidoSemComponentesFrete), value: this.FreteLiquidoSemComponentesFrete },
            { text: this.obterDescricao(this.TotalReceber), value: this.TotalReceber },
            { text: this.obterDescricao(this.ICMS), value: this.ICMS },
            { text: this.obterDescricao(this.ICMSST), value: this.ICMSST },
            { text: this.obterDescricao(this.PIS), value: this.PIS },
            { text: this.obterDescricao(this.COFINS), value: this.COFINS },
            { text: this.obterDescricao(this.ISS), value: this.ISS },
            { text: this.obterDescricao(this.ISSRetido), value: this.ISSRetido },
            { text: this.obterDescricao(this.FreteValor), value: this.FreteValor },
            { text: this.obterDescricao(this.TaxaDescarga), value: this.TaxaDescarga },
            { text: this.obterDescricao(this.TotalReceberSemISS), value: this.TotalReceberSemISS },
            { text: this.obterDescricao(this.CBS), value: this.CBS },
            { text: this.obterDescricao(this.IBSMunicipal), value: this.IBSMunicipal },
            { text: this.obterDescricao(this.IBSEstadual), value: this.IBSEstadual }
        ];
    },
    obterOpcoesIVA: function () {
        return [].concat(this.obterOpcoes()).concat([
            { text: this.obterDescricao(this.AdValorem), value: this.AdValorem },
            { text: this.obterDescricao(this.GRIS), value: this.GRIS },
            { text: this.obterDescricao(this.Pedagio), value: this.Pedagio },
            { text: this.obterDescricao(this.TaxaEntrega), value: this.TaxaEntrega },
            { text: this.obterDescricao(this.ImpostoValorAgregado), value: this.ImpostoValorAgregado },
            { text: this.obterDescricao(this.Pernoite), value: this.Pernoite },
            { text: this.obterDescricao(this.CustoFixo), value: this.CustoFixo },
            { text: this.obterDescricao(this.FreteCaixa), value: this.FreteCaixa },
            { text: this.obterDescricao(this.FreteKM), value: this.FreteKM },
            { text: this.obterDescricao(this.FretePeso), value: this.FretePeso },
            { text: this.obterDescricao(this.FreteViagem), value: this.FreteViagem },
            { text: this.obterDescricao(this.TaxaTotal), value: this.TaxaTotal }
        ]);
    },
    obterOpcoesIVAPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoesIVA());
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoContaContabil = Object.freeze(new EnumTipoContaContabilHelper());
