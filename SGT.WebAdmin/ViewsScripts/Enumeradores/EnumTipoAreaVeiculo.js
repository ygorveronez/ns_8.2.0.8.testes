var EnumTipoAreaVeiculoHelper = function () {
    this.Todos = "";
    this.Doca = 1;
    this.Patio = 2;
};

EnumTipoAreaVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAreaVeiculo.Doca, value: this.Doca },
            { text: Localization.Resources.Enumeradores.TipoAreaVeiculo.Patio, value: this.Patio }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoAreaVeiculo.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoAreaVeiculo = Object.freeze(new EnumTipoAreaVeiculoHelper());