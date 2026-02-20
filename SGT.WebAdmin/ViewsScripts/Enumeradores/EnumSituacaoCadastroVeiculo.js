var EnumSituacaoCadastroVeiculoHelper = function () {
    this.Todos = 0;
    this.Aprovado = 1;
    this.Pendente = 2;
    this.Rejeitada = 3;
    this.SemRegraAprovacao = 4;
};

EnumSituacaoCadastroVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aprovado", value: this.Aprovado },
            { text: "Pendente", value: this.Pendente },
            { text: "Rejeitada", value: this.Rejeitada },
            { text: "Sem Regra", value: this.SemRegraAprovacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoCadastroVeiculo = Object.freeze(new EnumSituacaoCadastroVeiculoHelper());