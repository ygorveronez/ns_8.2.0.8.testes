var EnumTipoEntidadeProgramacaoHelper = function () {
    this.Todos = 0;
    this.Veiculo = 1;
    this.Motorista = 2;
};

EnumTipoEntidadeProgramacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Veículo", value: this.Veiculo },
            { text: "Motorista", value: this.Motorista }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Veículo", value: this.Veiculo },
            { text: "Motorista", value: this.Motorista }
        ];
    }
};

var EnumTipoEntidadeProgramacao = Object.freeze(new EnumTipoEntidadeProgramacaoHelper());