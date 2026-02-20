var EnumOrdenacaoHelper = function () {
    this.Crescente = 'asc';
    this.Decrescente = 'desc';
};

EnumOrdenacaoHelper.prototype.ObterOpcoes = function () {
    return [
        { text: Localization.Resources.Enumeradores.Ordenacao.Crescente, value: this.Crescente },
        { text: Localization.Resources.Enumeradores.Ordenacao.Decrescente, value: this.Decrescente }
    ];
};

var EnumOrdenacao = Object.freeze(new EnumOrdenacaoHelper());