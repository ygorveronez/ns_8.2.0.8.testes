var EnumTipoPropriedadeVeiculoTipoOperacaoHelper = function () {
    this.SomenteFrotaPropria = 1;
    this.SomenteTerceiros = 2;
    this.Ambos = 3;
}

EnumTipoPropriedadeVeiculoTipoOperacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ambos", value: this.Ambos },
            { text: "Somente frota própria", value: this.SomenteFrotaPropria },
            { text: "Somente terceiros", value: this.SomenteTerceiros }
        ];
    },
}

var EnumTipoPropriedadeVeiculoTipoOperacao = Object.freeze(new EnumTipoPropriedadeVeiculoTipoOperacaoHelper());