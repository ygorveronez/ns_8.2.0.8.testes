var EnumTupoUsoFatorCubagemHelper = function () {
    this.ApenasQuandoMaiorQuePesoDaMercadoria = 0;
    this.SempreUtilizarPesoConvertido = 1;
};

EnumTupoUsoFatorCubagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TupoUsoFatorCubagem.ApenasQuandoMaiorQuePesoDaMercadoria, value: this.ApenasQuandoMaiorQuePesoDaMercadoria },
            { text: Localization.Resources.Enumeradores.TupoUsoFatorCubagem.SempreUtilizarPesoConvertido, value: this.SempreUtilizarPesoConvertido }
        ];
    },
};

var EnumTupoUsoFatorCubagem = Object.freeze(new EnumTupoUsoFatorCubagemHelper());