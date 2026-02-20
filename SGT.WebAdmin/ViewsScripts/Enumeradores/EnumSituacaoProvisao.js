var EnumSituacaoProvisaoHelper = function () {
    this.Todas = "";
    this.Todos = 0;
    this.EmAlteracao = 1;
    this.AgIntegracao = 2;
    this.FalhaIntegracao = 3;
    this.Finalizado = 4;
    this.Cancelado = 5;
    this.EmIntegracao = 6;
    this.EmFechamento = 7;
    this.PendenciaFechamento = 8
};

EnumSituacaoProvisaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Alteração", value: this.EmAlteracao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Falha na Integração", value: this.FalhaIntegracao },
            { text: "Pendência no Fechamento", value: this.PendenciaFechamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }, { text: "Em Fechamento", value: this.EmFechamento }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoProvisao = Object.freeze(new EnumSituacaoProvisaoHelper());