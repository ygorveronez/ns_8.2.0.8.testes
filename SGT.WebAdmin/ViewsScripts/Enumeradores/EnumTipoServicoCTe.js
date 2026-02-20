var EnumTipoServicoCTeHelper = function () {
    this.Todos = -1;
    this.Normal = 0;
    this.SubContratacao = 1;
    this.Redespacho = 2;
    this.RedespachoIntermediario = 3;
    this.VinculadoMultimodal = 4;

    this.TransporteDePessoas = 6;
    this.TransporteDeValores = 7;
    this.ExcessoDeBagagem = 8;
};

EnumTipoServicoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.Subcontratacao, value: this.SubContratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.Redespacho, value: this.Redespacho },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.RedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.VinculadoMultimodal, value: this.VinculadoMultimodal }
        ];
    },
    obterOpcoesComInternas: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.Subcontratacao, value: this.SubContratacao },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.Redespacho, value: this.Redespacho },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.RedespachoIntermediario, value: this.RedespachoIntermediario },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.VinculadoMultimodal, value: this.VinculadoMultimodal },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.TransporteDePessoas, value: this.TransporteDePessoas },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.TransporteDeValores, value: this.TransporteDeValores },
            { text: Localization.Resources.Enumeradores.TipoServicoCTe.ExcessoDeBagagem, value: this.ExcessoDeBagagem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoServicoCTe.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoServicoCTe = Object.freeze(new EnumTipoServicoCTeHelper());