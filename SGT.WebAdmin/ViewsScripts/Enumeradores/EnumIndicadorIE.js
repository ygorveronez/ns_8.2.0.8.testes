var EnumIndicadorIEHelper = function () {
    this.UmContribuinteICMSInformarIEDoDestinatario = 1;
    this.DoisContribuinteIsentoDeInscricaoNoCadastroDeContribuintesDoICMS = 2;
    this.NoveNaoContribuinteQuePodeOuNaoPossuirInscricaoEstadualNoCadastroDeContribuintesDoICMS = 9;
    this.NaoInformado = "";
};

EnumIndicadorIEHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.IndicadorIE.UmContribuinteICMSInformarIEDoDestinatario, value: this.UmContribuinteICMSInformarIEDoDestinatario },
            { text: Localization.Resources.Enumeradores.IndicadorIE.DoisContribuinteIsentoDeInscricaoNoCadastroDeContribuintesDoICMS, value: this.DoisContribuinteIsentoDeInscricaoNoCadastroDeContribuintesDoICMS },
            { text: Localization.Resources.Enumeradores.IndicadorIE.NoveNaoContribuinteQuePodeOuNaoPossuirInscricaoEstadualNoCadastroDeContribuintesDoICMS, value: this.NoveNaoContribuinteQuePodeOuNaoPossuirInscricaoEstadualNoCadastroDeContribuintesDoICMS },
            { text: Localization.Resources.Enumeradores.IndicadorIE.NaoInformado, value: this.NaoInformado }
        ];
    },
};

var EnumIndicadorIE = Object.freeze(new EnumIndicadorIEHelper());