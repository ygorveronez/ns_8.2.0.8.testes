var EnumSituacaoProspeccaoHelper = function () {
    this.Todos = "";
    this.Pendente = 0;
    this.Vendido = 1;
    this.NaoVendido = 2;
    this.PlanejamentoPreparacao = 3;
    this.LevantamentoNecessidades = 4;
    this.Proposta = 5;
    this.Negociacao = 6;
    this.Fechado = 7;
    this.NaoFechado = 8;
    this.PosVenda = 9;
};

EnumSituacaoProspeccaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendentes", value: this.Pendente },
            { text: "Vendido", value: this.Vendido },
            { text: "Não Vendido", value: this.NaoVendido },
            { text: "Planejamento Preparação", value: this.PlanejamentoPreparacao },
            { text: "Levantamento Necessidades", value: this.LevantamentoNecessidades },
            { text: "Proposta", value: this.Proposta },
            { text: "Negociação", value: this.Negociacao },
            { text: "Fechado", value: this.Fechado },
            { text: "Não Fechado", value: this.NaoFechado },
            { text: "Pós-venda", value: this.PosVenda },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoProspeccao = Object.freeze(new EnumSituacaoProspeccaoHelper());