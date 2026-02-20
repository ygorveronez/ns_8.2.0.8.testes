var EnumTipoColetaEntregaDevolucaoHelper = function () {
    this.Total = 0;
    this.Parcial = 1;
};

EnumTipoColetaEntregaDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoColetaEntregaDevolucao.Parcial, value: this.Parcial },
            { text: Localization.Resources.Enumeradores.TipoColetaEntregaDevolucao.Total, value: this.Total }
        ];
    },
};

var EnumTipoColetaEntregaDevolucao = Object.freeze(new EnumTipoColetaEntregaDevolucaoHelper());
