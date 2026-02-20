var EnumTipoPesquisaHelper = function () {
    this.Carga = 1;
    this.NotaFiscal = 2;
    this.Pedido = 3;
};

EnumTipoPesquisaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoConsulta.Carga, value: this.Carga },
            { text: Localization.Resources.Enumeradores.TipoConsulta.NotaFiscal, value: this.NotaFiscal },
            { text: Localization.Resources.Enumeradores.TipoConsulta.Pedido, value: this.Pedido }
        ];
    },
}

var EnumTipoPesquisa = Object.freeze(new EnumTipoPesquisaHelper());