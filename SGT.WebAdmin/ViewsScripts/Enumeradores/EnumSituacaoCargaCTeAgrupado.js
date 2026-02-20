var EnumSituacaoCargaCTeAgrupadoHelper = function () {
    this.EmEmissao = 0;
    this.Rejeitado = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
    this.EmCancelamento = 4;
    this.AgIntegracao = 5;
    this.FalhaIntegracao = 6;
};

EnumSituacaoCargaCTeAgrupadoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Em Emissão", value: this.EmEmissao },
            { text: "Rejeitado", value: this.Rejeitado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Em cancelamento", value: this.EmCancelamento },
            { text: "Aguardando Integrações", value: this.AgIntegracao },
            { text: "Falha em integração", value: this.FalhaIntegracao }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoCargaCTeAgrupado = Object.freeze(new EnumSituacaoCargaCTeAgrupadoHelper());