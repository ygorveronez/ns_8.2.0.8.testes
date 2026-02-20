var EnumInformacaoManuseioHelper = function () {
    this.Todos = "";
    this.Im01 = 1;
    this.Im02 = 2;
    this.Im03 = 3;
    this.Im04 = 4;
    this.Im05 = 5;
    this.Im06 = 6;
    this.Im07 = 7;
    this.Im08 = 8;
    this.Im09 = 9;
    this.Im10 = 10;
    this.Im11 = 11;
    this.Im12 = 12;
    this.Im13 = 13;
    this.Im14 = 14;
    this.Im15 = 15;
    this.Im99 = 99;
};

EnumInformacaoManuseioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.UmCertificadoDoExpedidorParaEmbarqueDeAnimalVivo, value: this.Im01 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.DoisArtigoPerigosoConformeDeclaracaoDoExpedidorAnexa, value: this.Im02 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.TresSomenteEmAeronaveCargueira, value: this.Im03 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.QuatroArtigoPerigosoDeclaracaoDoExpedidorNaoRequerida, value: this.Im04 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.CincoArtigoPerigosoEmQuantidadeIsenta, value: this.Im05 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.SeisGeloSecoParaRefrigeracaoEspecificarNoCampoObservacoesQuantidade, value: this.Im06 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.SeteNaoRestritoEspecificarDisposicaoEspecialNoCampoObservacoes, value: this.Im07 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.OitoArtigoPerigosoEmCargaConsolidadaEspecificarQuantidadeNoCampoObservacoes, value: this.Im08 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.NoveAutorizacaoDaAutoridadeGovernamentalAnexaEspecificarNoCampoObservacoes, value: this.Im09 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.DezBateriasDeIonsDeLitioEmConformidadeComSecaoIIDaPI965CAO, value: this.Im10 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.OnzeBateriasDeIonsDeLitioEmConformidadeComSecaoIIDaPI966, value: this.Im11 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.DozeBateriasDeIonsDeLitioEmConformidadeComSecaoIIDaPI967, value: this.Im12 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.TrezeBateriasDeMetalLitioEmConformidadeComSecaoIIDaPI968CAO, value: this.Im13 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.QuatorzeBateriasDeMetalLitioEmConformidadeComSecaoIIDaPI969, value: this.Im14 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.QuinzeBateriasDeMetalLitioEmConformidadeComSeçaoIIDaPI970, value: this.Im15 },
            { text: Localization.Resources.Enumeradores.InformacaoManuseio.NoventaNoveOutroEspecificarNoCampoObservacoes, value: this.Im99 }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.InformacaoManuseio.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumInformacaoManuseio = Object.freeze(new EnumInformacaoManuseioHelper());