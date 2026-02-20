const EnumTipoLicencaHelper = function () {
    this.Todos = -1;
    this.Geral = 0;
    this.Motorista = 1;
    this.Veiculo = 2;
    this.Pessoa = 3;
    this.Tracao = 4;
    this.Reboque = 5;
};

EnumTipoLicencaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Geral", value: this.Geral },
            { text: "Motorista", value: this.Motorista },
            { text: "Veículo", value: this.Veiculo },
            { text: "Pessoa", value: this.Pessoa },
            { text: "Tração", value: this.Tracao },
            { text: "Reboque", value: this.Reboque }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todas }].concat(this.obterOpcoes());
    }
};

const EnumTipoLicenca = Object.freeze(new EnumTipoLicencaHelper());
