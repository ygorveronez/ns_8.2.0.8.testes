var EnumSituacaoReformaPalletHelper = function () {
    this.Todas = 0;
    this.AguardandoNfeSaida = 1;
    this.AguardandoRetorno = 2;
    this.CanceladaNfeSaida = 3;
    this.CanceladaRetorno = 4;
    this.Finalizada = 5;
}

EnumSituacaoReformaPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando NF-e de Saída", value: this.AguardandoNfeSaida },
            { text: "Aguardando Retorno", value: this.AguardandoRetorno },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Cancelada (NF-e de Saída)", value: this.CanceladaNfeSaida },
            { text: "Cancelada (Retorno)", value: this.CanceladaRetorno },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoReformaPallet = Object.freeze(new EnumSituacaoReformaPalletHelper());