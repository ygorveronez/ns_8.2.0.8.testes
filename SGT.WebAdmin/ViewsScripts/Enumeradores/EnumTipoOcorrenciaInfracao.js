var EnumTipoOcorrenciaInfracaoHelper = function () {
    this.Veiculo = 0;
    this.Funcionario= 1;
    this.Motorista = 2;
};

EnumTipoOcorrenciaInfracaoHelper.prototype.ObterOpcoes = function () {
    return [
        { value: this.Veiculo, text: "Veículo" },
        { value: this.Funcionario, text: "Funcionário" },
        { value: this.Motorista, text: "Motorista" }
    ];
};

EnumTipoOcorrenciaInfracaoHelper.prototype.ObterOpcoesPesquisa = function () {
    return [
        { value: "", text: "Todos" },
        { value: this.Veiculo, text: "Veículo" },
        { value: this.Funcionario, text: "Funcionário" },
        { value: this.Motorista, text: "Motorista" }
    ];
};

var EnumTipoOcorrenciaInfracao = Object.freeze(new EnumTipoOcorrenciaInfracaoHelper());