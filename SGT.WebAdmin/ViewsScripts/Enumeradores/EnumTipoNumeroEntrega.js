var EnumTipoNumeroEntregaHelper = function () {
    this.PorFaixaEntrega = 1;
    this.ValorFixoPorEntrega = 2;

};

EnumTipoNumeroEntregaHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoNumeroEntrega.PorFaixaEntrega, value: this.PorFaixaEntrega },
            { text: Localization.Resources.Enumeradores.TipoNumeroEntrega.ValorFixoPorEntrega, value: this.ValorFixoPorEntrega },
        ];
        return arrayOpcoes;
    }
};

var EnumTipoNumeroEntrega = Object.freeze(new EnumTipoNumeroEntregaHelper());