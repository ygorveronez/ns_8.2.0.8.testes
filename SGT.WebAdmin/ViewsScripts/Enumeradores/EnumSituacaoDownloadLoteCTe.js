var EnumSituacaoDownloadLoteCTeHelper = function () {
    this.Pendente = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
    this.Falha = 4;
}

EnumSituacaoDownloadLoteCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Falha", value: this.Falha },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoDownloadLoteCTe = Object.freeze(new EnumSituacaoDownloadLoteCTeHelper());