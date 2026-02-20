var EnumSituacaoLoteEscrituracaoCancelamentoHelper = function () {
    this.EmCriacao = 1;
    this.AgIntegracao = 2;
    this.FalhaIntegracao = 3;
    this.Finalizado = 4;
    this.Cancelado = 5;
};

EnumSituacaoLoteEscrituracaoCancelamentoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.EmCriacao, text: "Em Criação" },
            { value: this.AgIntegracao, text: "Ag. Integração" },
            { value: this.FalhaIntegracao, text: "Falha na Integração" },
            { value: this.Finalizado, text: "Finalizado" },
            { value: this.Cancelado, text: "Cancelado" }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ value: "", text: "Todos" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoLoteEscrituracaoCancelamento = Object.freeze(new EnumSituacaoLoteEscrituracaoCancelamentoHelper());