var EnumNivelQuebraProdutoRoteirizarHelper = function () {
    this.Item = 0;
    this.Caixa = 1;
}

EnumNivelQuebraProdutoRoteirizarHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.NivelQuebraProdutoRoteirizar.Item, value: this.Item },
            { text: Localization.Resources.Enumeradores.NivelQuebraProdutoRoteirizar.Caixa, value: this.Caixa }
        ];
    }
}

var EnumNivelQuebraProdutoRoteirizar = Object.freeze(new EnumNivelQuebraProdutoRoteirizarHelper());