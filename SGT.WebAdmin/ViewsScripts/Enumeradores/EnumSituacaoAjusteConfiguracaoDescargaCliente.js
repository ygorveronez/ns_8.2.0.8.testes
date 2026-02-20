var EnumSituacaoAjusteConfiguracaoDescargaClienteHelper = function () {
    this.AgAprovacao = 0;
    this.SemRegraAprovacao = 1;
    this.RejeitadaAutorizacao = 2;
    this.Aprovada = 3;
    this.Todos = 9;
};

EnumSituacaoAjusteConfiguracaoDescargaClienteHelper.prototype = {
    obterOpcoes: function () {
        return [            
            { text: "Aguardando Aprovação", value: this.AgAprovacao },
            { text: "Sem Regra Aprovação", value: this.SemRegraAprovacao },
            { text: "Rejeitada Autorização", value: this.RejeitadaAutorizacao },
            { text: "Aprovada", value: this.Aprovada },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoAjusteConfiguracaoDescargaCliente = Object.freeze(new EnumSituacaoAjusteConfiguracaoDescargaClienteHelper());