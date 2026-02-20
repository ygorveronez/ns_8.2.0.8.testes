var EnumPadraoEixosVeiculoHelper = function () {
    this.Simples = 1;
    this.Duplo = 2;
};

EnumPadraoEixosVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.PadraoEixosVeiculo.Simples, value: this.Simples },
            { text: Localization.Resources.Enumeradores.PadraoEixosVeiculo.Duplo, value: this.Duplo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.PadraoEixosVeiculo.Todos, value: "" }].concat(this.obterOpcoes());
    }
}

var EnumPadraoEixosVeiculo = Object.freeze(new EnumPadraoEixosVeiculoHelper());