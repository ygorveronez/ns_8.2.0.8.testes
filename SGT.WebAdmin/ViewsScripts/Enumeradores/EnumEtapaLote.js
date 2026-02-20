var EnumEtapaLoteHelper = function () {
    this.Todas = 0;
    this.CriacaoLote = 1;
    this.AutorizacaoLote = 2;
    this.IntegracaoLote = 3;
    this.Integrado = 4;
    this.Finalizado = 5;
};

EnumEtapaLoteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Autorização Lote", value: this.AutorizacaoLote },
            { text: "Criação Lote", value: this.CriacaoLote },
            { text: "Integração Lote", value: this.IntegracaoLote },
            { text: "Integrado", value: this.Integrado },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumEtapaLote = Object.freeze(new EnumEtapaLoteHelper());