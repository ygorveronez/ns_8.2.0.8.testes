var EnumVeiculoEquipamentoHelper = function () {
    this.Todos = 0;
    this.Veiculo = 1;
    this.Equipamento = 2;
};

EnumVeiculoEquipamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Veículo", value: this.Veiculo },
            { text: "Equipamento", value: this.Equipamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumVeiculoEquipamento = Object.freeze(new EnumVeiculoEquipamentoHelper());