var EnumTipoQuitacaoCIOTHelper = function () {
    this.Padrao = 0;
    this.QualquerLugar = 1;
    this.ETF = 2;
    this.Filial = 3;
    this.Transportadora = 4;
};

EnumTipoQuitacaoCIOTHelper.prototype = {
    ObterDescricao: function (TipoQuitacaoCIOT) {
        switch (TipoQuitacaoCIOT) {
            case this.Padrao: return Localization.Resources.Enumeradores.TipoQuitacaoCIOT.Padrao;
            case this.QualquerLugar: return Localization.Resources.Enumeradores.QualquerLugar;
            case this.ETF: return Localization.Resources.Enumeradores.TipoQuitacaoCIOT.ETF;
            case this.Filial: return Localization.Resources.Enumeradores.Filial;
            case this.Transportadora: return Localization.Resources.TipoQuitacaoCIOT.Transportadora;
            default: return "";
        }
    },
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoQuitacaoCIOT.Padrao, value: this.Padrao },
            { text: Localization.Resources.Enumeradores.TipoQuitacaoCIOT.QualquerLugar, value: this.QualquerLugar },
            { text: Localization.Resources.Enumeradores.TipoQuitacaoCIOT.ETF, value: this.ETF },
            { text: Localization.Resources.Enumeradores.TipoQuitacaoCIOT.Filial, value: this.Filial },
            { text: Localization.Resources.Enumeradores.TipoQuitacaoCIOT.Transportadora, value: this.Transportadora }
        ];
    }
};

var EnumTipoQuitacaoCIOT = Object.freeze(new EnumTipoQuitacaoCIOTHelper());