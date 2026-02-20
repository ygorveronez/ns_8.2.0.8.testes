var EnumTipoTelaLicencaHelper = function () {
    this.Todos = 0;
    this.Funcionario = 1;
    this.Motorista = 2;
    this.Pessoa = 3;
    this.Veiculo = 4;
};

EnumTipoTelaLicencaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Funcionário", value: this.Funcionario },
            { text: "Motorista", value: this.Motorista },
            { text: "Pessoa", value: this.Pessoa },
            { text: "Veículo", value: this.Veiculo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTelaLicenca = Object.freeze(new EnumTipoTelaLicencaHelper());