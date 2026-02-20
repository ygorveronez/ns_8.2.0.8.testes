var EnumTipoContratacaoCargaHelper = function () {
    this.Normal = 0;
    this.SubContratada = 2;
    this.NormalESubContratada = 3;
    this.Redespacho = 4;
    this.RedespachoIntermediario = 5;
    this.SVMProprio = 6;
    this.SVMTerceiro = 7;
};

EnumTipoContratacaoCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.Subcontratada, value: this.SubContratada },
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.NormalSubcontratada, value: this.NormalESubContratada },
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.Redespacho, value: this.Redespacho },
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.SVMProprio, value: this.SVMProprio },
            { text: Localization.Resources.Enumeradores.TipoContratacaoCarga.SVMTerceiro, value: this.SVMTerceiro }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoContratacaoCarga.Todos, value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoContratacaoCarga = Object.freeze(new EnumTipoContratacaoCargaHelper());