var EnumSituacaoLoteHelper = function () {
    this.Todas = 8;
    this.EmCriacao = 1;
    this.AgAprovacao = 2;
    this.Reprovacao = 3;
    this.EmCorrecao = 4;
    this.AgIntegracao = 5;
    this.IntegracaoReprovada = 6;
    this.Finalizada = 7;
    this.Integrada = 9;
    this.AgAprovacaoIntegracao = 10;
    this.FalhaIntegracao = 11;
    this.EmIntegracao = 12;
    this.FinalizadaComDestino = 13;
};

EnumSituacaoLoteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Aprovação Integração", value: this.AgAprovacaoIntegracao },
            { text: "Ag. Aprovação Transportador", value: this.AgAprovacao },
            { text: "Em Criação", value: this.EmCriacao },
            { text: "Em Correção", value: this.EmCorrecao },
            { text: "Reprovação", value: this.Reprovacao },
            { text: "Integração Reprovada", value: this.IntegracaoReprovada },
            { text: "Integrada", value: this.Integrada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Finalizada com Destino", value: this.FinalizadaComDestino }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoLote = Object.freeze(new EnumSituacaoLoteHelper());