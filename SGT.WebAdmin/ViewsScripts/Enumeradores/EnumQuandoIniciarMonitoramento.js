var EnumQuandoIniciarMonitoramentoHelper = function () {
    this.NaoIniciar = 1;
    this.AoGerarCarga = 2;
    this.AoInformarVeiculoNaCarga = 3;
    this.AoInformarVeiculoNaCargaECargaEmTransporte = 4;
    this.AoIniciarViagem = 5;
    this.EstouIndoOuAoIniciarViagem = 6;
};

EnumQuandoIniciarMonitoramentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.NaoIniciar,
                text: "Não iniciar"
            }, {
                value: this.AoGerarCarga,
                text: "Ao gerar carga"
            }, {
                value: this.AoInformarVeiculoNaCarga,
                text: "Ao informar o veículo na carga"
            }, {
                value: this.AoInformarVeiculoNaCargaECargaEmTransporte,
                text: "Ao informar o veículo na carga e carga estiver em transporte"
            }, {
                value: this.AoIniciarViagem,
                text: "Ao iniciar Viagem (manualmente ou via APP)"
            }, {
                value: this.EstouIndoOuAoIniciarViagem,
                text: "Ao receber a ação do \"Estou Indo\" do App ou inicio de Viagem"
            }
        ];
    }
}

var EnumQuandoIniciarMonitoramento = Object.freeze(new EnumQuandoIniciarMonitoramentoHelper());
