var EnumUnidadeCapacidadeHelper = function () {
    this.Peso = 1;
    this.Unidade = 2;
};

 EnumUnidadeCapacidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.UnidadeCapacidade.Peso, value: this.Peso },
            { text: Localization.Resources.Enumeradores.UnidadeCapacidade.Unidade, value: this.Unidade}
        ];
    },
}

var EnumUnidadeCapacidade = Object.freeze(new EnumUnidadeCapacidadeHelper());