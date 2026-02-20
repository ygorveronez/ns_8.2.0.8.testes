var EnumTipoRoteirizacaoColetaEntregaHelper = function () {
    this.Entrega = 0;
    this.ColetaEntrega = 1;
    this.Coleta = 2;
}

EnumTipoRoteirizacaoColetaEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoRoteirizacaoColetaEntrega.Coleta, value: this.Coleta },
            { text: Localization.Resources.Enumeradores.TipoRoteirizacaoColetaEntrega.ColetaEntrega, value: this.ColetaEntrega },
            { text: Localization.Resources.Enumeradores.TipoRoteirizacaoColetaEntrega.Entrega, value: this.Entrega }
        ];
    }
}

var EnumTipoRoteirizacaoColetaEntrega = Object.freeze(new EnumTipoRoteirizacaoColetaEntregaHelper());