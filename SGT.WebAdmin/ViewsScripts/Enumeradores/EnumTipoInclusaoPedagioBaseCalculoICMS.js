var EnumTipoInclusaoPedagioBaseCalculoICMSHelper = function() {
    this.UtilizarPadrao = 0;
    this.SempreIncluir = 1;
    this.NuncaIncluir = 2;
};

EnumTipoInclusaoPedagioBaseCalculoICMSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoInclusaoPedagioBaseCalculoICMS.UtilizarPadrao, value: this.UtilizarPadrao },
            { text: Localization.Resources.Enumeradores.TipoInclusaoPedagioBaseCalculoICMS.SempreIncluir, value: this.SempreIncluir },
            { text: Localization.Resources.Enumeradores.TipoInclusaoPedagioBaseCalculoICMS.NuncaIncluir, value: this.NuncaIncluir }
        ];
    }
};

var EnumTipoInclusaoPedagioBaseCalculoICMS = Object.freeze(new EnumTipoInclusaoPedagioBaseCalculoICMSHelper());