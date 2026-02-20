var EnumStatusLicencaHelper = function () {
    this.Todas = "";
    this.Vigente = 1;
    this.Vencido = 2;
    this.Aprovado = 3;
    this.Reprovado = 4;
};

EnumStatusLicencaHelper.prototype = {
    obterDescricao: function (status) {
        switch (status) {
            case this.Vigente: return Localization.Resources.Enumeradores.StatusLicenca.Vigente;
            case this.Vencido: return Localization.Resources.Enumeradores.StatusLicenca.Vencido;
            case this.Aprovado: return Localization.Resources.Enumeradores.StatusLicenca.Aprovado;
            case this.Reprovado: return Localization.Resources.Enumeradores.StatusLicenca.Reprovado;
            default: return undefined;
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusLicenca.Vigente, value: this.Vigente },
            { text: Localization.Resources.Enumeradores.StatusLicenca.Vencido, value: this.Vencido }
        ];
    },
    obterOpcoesEmbarcador: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusLicenca.Aprovado, value: this.Aprovado },
            { text: Localization.Resources.Enumeradores.StatusLicenca.Reprovado, value: this.Reprovado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.StatusLicenca.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaEmbarcador: function () {
        return [{ text: Localization.Resources.Enumeradores.StatusLicenca.Todas, value: this.Todas }].concat(this.obterOpcoesEmbarcador());
    }
};

var EnumStatusLicenca = Object.freeze(new EnumStatusLicencaHelper());
