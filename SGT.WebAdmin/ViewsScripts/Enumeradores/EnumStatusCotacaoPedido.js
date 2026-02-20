var EnumStatusCotacaoPedidoHelper = function () {
    this.Todos = 0;
    this.Fechada = 1;
    this.PerdaPorPreco = 2;
    this.PerdaPorDesistenciaDoServico = 3;
    this.PerdaPorNaoJustificativaPeloCliente = 4;
    this.PerdaPorPrazoDeResposta = 5;
    this.PertaPorQualificacaoDocumental = 6;
    this.PerdaPorQualificacaoTecnica = 7;
    this.PerdaPorInfraestrutura = 8;
    this.PerdaPorAnaliseCadastral = 9;
    this.EmAnalise = 10;
    this.Sondagem = 11;
};

EnumStatusCotacaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fechada", value: this.Fechada },
            { text: "Perda por preço", value: this.PerdaPorPreco },
            { text: "Perda por desistência do serviço", value: this.PerdaPorDesistenciaDoServico },
            { text: "Perda por não justificativa pelo cliente", value: this.PerdaPorNaoJustificativaPeloCliente },
            { text: "Perda por prazo de resposta", value: this.PerdaPorPrazoDeResposta },
            { text: "Perda por qualificação documental", value: this.PertaPorQualificacaoDocumental },
            { text: "Perda por qualificação técnica", value: this.PerdaPorQualificacaoTecnica },
            { text: "Perda por infraestrutura", value: this.PerdaPorInfraestrutura },
            { text: "Perda por análise cadastral", value: this.PerdaPorAnaliseCadastral },
            { text: "Em análise", value: this.EmAnalise },
            { text: "Sondagem", value: this.Sondagem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Fechada", value: this.Fechada },
            { text: "Perda por preço", value: this.PerdaPorPreco },
            { text: "Perda por desistência do serviço", value: this.PerdaPorDesistenciaDoServico },
            { text: "Perda por não justificativa pelo cliente", value: this.PerdaPorNaoJustificativaPeloCliente },
            { text: "Perda por prazo de resposta", value: this.PerdaPorPrazoDeResposta },
            { text: "Perda por qualificação documental", value: this.PertaPorQualificacaoDocumental },
            { text: "Perda por qualificação técnica", value: this.PerdaPorQualificacaoTecnica },
            { text: "Perda por infraestrutura", value: this.PerdaPorInfraestrutura },
            { text: "Perda por análise cadastral", value: this.PerdaPorAnaliseCadastral },
            { text: "Em análise", value: this.EmAnalise },
            { text: "Sondagem", value: this.Sondagem }
        ];
    }
};

var EnumStatusCotacaoPedido = Object.freeze(new EnumStatusCotacaoPedidoHelper());