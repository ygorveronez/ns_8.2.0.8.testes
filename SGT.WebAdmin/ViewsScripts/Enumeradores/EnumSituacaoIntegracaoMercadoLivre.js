var EnumSituacaoIntegracaoMercadoLivreHelper = function () {
    this.Todas = 99;
    this.PendenteDownload = 0;
    this.PendenteProcessamento = 1;
    this.Concluido = 2;
    this.Desconsiderado = 3;
};

EnumSituacaoIntegracaoMercadoLivreHelper.prototype = {
    obterOpcoes: function() {
        return [
            { text: "Pend. Download", value: this.PendenteDownload },
            { text: "Pend. Processamento", value: this.PendenteProcessamento },
            { text: "Concluído", value: this.Concluido },
            { text: "Desconsiderado", value: this.Desconsiderado }
        ];
    },
    obterOpcoesPesquisa: function() {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoIntegracaoMercadoLivre = Object.freeze(new EnumSituacaoIntegracaoMercadoLivreHelper());