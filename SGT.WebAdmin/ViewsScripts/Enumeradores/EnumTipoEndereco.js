EnumTipoEnderecoHelper = function () {
    this.Comercial = 1;
    this.Cobranca = 2;
    this.Residencial = 3;
    this.Outros = 99;
};

EnumTipoEnderecoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEndereco.Comercial, value: this.Comercial },
            { text: Localization.Resources.Enumeradores.TipoEndereco.Cobranca, value: this.Cobranca },
            { text: Localization.Resources.Enumeradores.TipoEndereco.Residencial, value: this.Residencial },
            { text: Localization.Resources.Enumeradores.TipoEndereco.Outros, value: this.Outros },
        ];
    },
};

var EnumTipoEndereco = Object.freeze(new EnumTipoEnderecoHelper());