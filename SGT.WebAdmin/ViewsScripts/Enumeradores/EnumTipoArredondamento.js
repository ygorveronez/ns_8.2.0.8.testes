var EnumTipoArredondamentoHelper = function () {
    this.PrimeiroItem = 0;
    this.UltimoItem = 1;
};

EnumTipoArredondamentoHelper.prototype.ObterOpcoes = function () {
    return [
        { value: this.PrimeiroItem, text: Localization.Resources.Enumeradores.TipoArredondamento.Primeiro },
        { value: this.UltimoItem, text: Localization.Resources.Enumeradores.TipoArredondamento.Ultimo }
    ];
};

var EnumTipoArredondamento = Object.freeze(new EnumTipoArredondamentoHelper());