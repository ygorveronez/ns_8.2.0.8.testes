var EnumFormaPagamentoChamadoHelper = function () {
    this.Todos = 0;
    this.PamcardMotorista = 1;
    this.ContaBancariaMotorista = 2;
    this.PamcardTerceiro = 3;
    this.ContaBancariaTerceiro = 4;
    this.Terceiro = 5;
};

EnumFormaPagamentoChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pamcard do Motorista", value: this.PamcardMotorista },
            { text: "Conta Bancária do Motorista", value: this.ContaBancariaMotorista },
            { text: "Pamcard de Terceiro", value: this.PamcardTerceiro },
            { text: "Conta Bancária de Terceiro", value: this.ContaBancariaTerceiro },
            { text: "Terceiro (reembolsar)", value: this.Terceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaPagamentoChamado = Object.freeze(new EnumFormaPagamentoChamadoHelper());