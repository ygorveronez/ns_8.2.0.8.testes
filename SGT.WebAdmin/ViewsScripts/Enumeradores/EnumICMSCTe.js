var EnumICMSCTeHelper = function () {
    this.Normal_00 = 1;
    this.ReducaoBaseCalculo_20 = 2;
    this.Isencao_40 = 3;
    this.NaoTributado_41 = 4;
    this.Diferido_51 = 5;
    this.CobradoPorSubstituicaoTributaria_60 = 6;
    this.OutrasSituacoes_90 = 9;
    this.DevidoAUFOrigemPrestacaoQuandoDiferenteUFEmitente_90 = 10;
    this.SimplesNacional = 11;
};

EnumICMSCTeHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.Normal_00, text: Localization.Resources.Enumeradores.ICMSCTe.ZeroNormal },
            { value: this.ReducaoBaseCalculo_20, text: Localization.Resources.Enumeradores.ICMSCTe.VinteComReducaoNaBaseDeCalculo },
            { value: this.Isencao_40, text: Localization.Resources.Enumeradores.ICMSCTe.QuarentaIsencao },
            { value: this.NaoTributado_41, text: Localization.Resources.Enumeradores.ICMSCTe.QuarentaUmNaoTributado },
            { value: this.Diferido_51, text: Localization.Resources.Enumeradores.ICMSCTe.CinquentaUmDiferido },
            { value: this.CobradoPorSubstituicaoTributaria_60, text: Localization.Resources.Enumeradores.ICMSCTe.SessentaCobradoPorSubstituicaoTributaria },
            { value: this.DevidoAUFOrigemPrestacaoQuandoDiferenteUFEmitente_90, text: Localization.Resources.Enumeradores.ICMSCTe.NoventaDevidoUFDeOrigemDaPrestacaoQuandoDiferenteDaUFDoEmitente },
            { value: this.OutrasSituacoes_90, text: Localization.Resources.Enumeradores.ICMSCTe.NoventaOutros },
            { value: this.SimplesNacional, text: Localization.Resources.Enumeradores.ICMSCTe.SimplesNacional }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ICMSCTe.Todos, value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumICMSCTe = Object.freeze(new EnumICMSCTeHelper());