var EnumContatosInformacoesEntregaTrizyHelper = function () {
    this.TelefoneTorre = 1;
    this.TelefoneDoCliente = 2;
};

EnumContatosInformacoesEntregaTrizyHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Pedidos.TipoOperacao.TelefoneTorre, value: this.TelefoneTorre },
            { text: Localization.Resources.Pedidos.TipoOperacao.TelefoneDoCliente, value: this.TelefoneDoCliente },
        ];
    }
};

var EnumContatosInformacoesEntregaTrizy = Object.freeze(new EnumContatosInformacoesEntregaTrizyHelper());