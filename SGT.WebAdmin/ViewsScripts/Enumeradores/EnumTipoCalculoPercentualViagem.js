var EnumTipoCalculoPercentualViagemHelper = function () {
    this.EntregasRealizadas = 1;
    this.ProximidadeEntrePosicaoVeiculoRotaPrevista = 2;
    this.DistanciaRotaPrevistaVersusDistanciaRotaRealizada = 3;
    this.DistanciaRotaRestanteAteDestino = 4;
};

EnumTipoCalculoPercentualViagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entregas realizadas", value: this.EntregasRealizadas },
            { text: "Proximidade entre posição do veículo e rota prevista", value: this.ProximidadeEntrePosicaoVeiculoRotaPrevista },
            { text: "Distância da rota prevista versus distância da rota realizada", value: this.DistanciaRotaPrevistaVersusDistanciaRotaRealizada },
            { text: "Distância da rota restante até destino", value: this.DistanciaRotaRestanteAteDestino }
        ];
    }
};

var EnumTipoCalculoPercentualViagem = Object.freeze(new EnumTipoCalculoPercentualViagemHelper());