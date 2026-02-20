var EnumTipoPropriedadeVeiculoHelper = function () {
    this.Todos = 'A';
    this.Proprio = 'P';
    this.Terceiros = 'T';
};

EnumTipoPropriedadeVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPropriedadeVeiculo.Proprio, value: this.Proprio },
            { text: Localization.Resources.Enumeradores.TipoPropriedadeVeiculo.Terceiros, value: this.Terceiros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPropriedadeVeiculo = Object.freeze(new EnumTipoPropriedadeVeiculoHelper());