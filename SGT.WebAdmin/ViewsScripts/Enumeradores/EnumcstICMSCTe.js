var EnumcstICMSCTeHelper = function () {
    this.MesmoDoCtEComplementado = "";
    this.TributacaoNormalIcms = "00";
    this.TributacaoComBcReduzidaDoIcms = 20;
    this.IcmsIsencao = 40;
    this.IcmsNaoTributada = 41;
    this.IcmsDiferido = 51;
    this.IcmsCobradoAnteriormentePorSubstituicaoTributaria = 60;
    this.IcmsDevidoAUfDeOrigemDaPrestacaoQuandoDiferenteDaUfDoEmitente = 90;
    this.IcmsOutros = 91;
    this.SimplesNacional = "SN";
};

EnumcstICMSCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.cstICMSCTe.TributacaoNormalIcms, value: this.TributacaoNormalIcms },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.TributacaoComBcReduzidaDoIcms, value: this.TributacaoComBcReduzidaDoIcms },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.IcmsIsencao, value: this.IcmsIsencao },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.IcmsNaoTributada, value: this.IcmsNaoTributada },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.IcmsDiferido, value: this.IcmsDiferido },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.IcmsCobradoAnteriormentePorSubstituicaoTributaria, value: this.IcmsCobradoAnteriormentePorSubstituicaoTributaria },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.IcmsDevidoAUfDeOrigemDaPrestacaoQuandoDiferenteDaUfDoEmitente, value: this.IcmsDevidoAUfDeOrigemDaPrestacaoQuandoDiferenteDaUfDoEmitente },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.IcmsOutros, value: this.IcmsOutros },
            { text: Localization.Resources.Enumeradores.cstICMSCTe.SimplesNacional, value: this.SimplesNacional }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.cstICMSCTe.MesmoDoCtEComplementado, value: this.MesmoDoCtEComplementado }].concat(this.obterOpcoes());
    }
};

var EnumcstICMSCTe = Object.freeze(new EnumcstICMSCTeHelper());
