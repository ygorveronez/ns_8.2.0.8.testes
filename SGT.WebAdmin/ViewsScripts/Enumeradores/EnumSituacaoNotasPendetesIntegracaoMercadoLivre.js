var EnumSituacaoNotasPendetesIntegracaoMercadoLivreHelper = function () {
    this.Pendente = 0,
    this.Concluido = 1,
    this.Falha = 2,
    this.Todas = 3
};

EnumSituacaoNotasPendetesIntegracaoMercadoLivreHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente Download", value: this.Pendente },
            { text: "Concluído", value: this.Concluido },
            { text: "Falha", value: this.Falha },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoNotasPendetesIntegracaoMercadoLivre = Object.freeze(new EnumSituacaoNotasPendetesIntegracaoMercadoLivreHelper());