var EnumResponsavelPagamentoInfracaoHelper = function () {
    this.Todos = "";
    this.Condutor = 1;
    this.Empresa = 2;
    this.Outro = 3;
    this.Funcionario = 4;
    this.AgregadoTerceiro = 5;
};

EnumResponsavelPagamentoInfracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Condutor", value: this.Condutor },
            { text: "Empresa", value: this.Empresa },
            { text: "Funcionário", value: this.Funcionario },
            { text: "Agregado/Terceiro", value: this.AgregadoTerceiro },
            { text: "Outro", value: this.Outro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumResponsavelPagamentoInfracao = Object.freeze(new EnumResponsavelPagamentoInfracaoHelper());