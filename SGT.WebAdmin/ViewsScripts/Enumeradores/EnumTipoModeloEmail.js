var EnumTipoModeloEmailHelper = function () {
    this.Todas = null;
    this.Padrao = 1;
    this.AgendamentoEntrega = 2;
    this.GestaoCustoContabilDevolucao = 3;
    this.ImprocedenciaCenarioPosEntregaDevolucao = 4;
    this.AgendamentoColeta = 5;
};

EnumTipoModeloEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "Agendamento de entrega", value: this.AgendamentoEntrega },
            { text: "Gestão de Custo e Contábil Devolução", value: this.GestaoCustoContabilDevolucao },
            { text: "Improcedência Cenário Pós Entrega Devolução", value: this.ImprocedenciaCenarioPosEntregaDevolucao },
            { text: "Agendamento de Coleta", value: this.AgendamentoColeta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumTipoModeloEmail = Object.freeze(new EnumTipoModeloEmailHelper());
