var EnumTipoInclusaoImpostoComplementoHelper = function () {
    this.ConformeCTeAnterior = 0;
    this.SempreIncluir = 1;
    this.NuncaIncluir = 2;
};

EnumTipoInclusaoImpostoComplementoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoInclusaoImpostoComplemento.ConformeCTeAnterior, value: this.ConformeCTeAnterior },
            { text: Localization.Resources.Enumeradores.TipoInclusaoImpostoComplemento.SempreIncluir, value: this.SempreIncluir },
            { text: Localization.Resources.Enumeradores.TipoInclusaoImpostoComplemento.NuncaIncluir, value: this.NuncaIncluir }
        ];
    }
}

var EnumTipoInclusaoImpostoComplemento = Object.freeze(new EnumTipoInclusaoImpostoComplementoHelper());