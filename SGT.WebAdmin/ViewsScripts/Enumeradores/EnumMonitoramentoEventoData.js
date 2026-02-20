var EnumMonitoramentoEventoDataHelper = function () {
    this.Padrao = 0;
    this.PosicaoVeiculo = 1;
    this.EntradaCliente = 2;
    this.SaidaCliente = 3;
    this.JanelaCarregamento = 4;
    this.JanelaDescarregamento = 5;
    this.EntregaPedidoPrevista = 6;
    this.InicioEntregaPrevista = 7;
    this.FinalEntregaPrevista = 8;
    this.InicioEntregaReprogramada = 9;
    this.FinalEntregaReprogramada = 10;
    this.InicioEntregaRealizada = 11;
    this.FinalEntregaRealizada = 12;
    this.DataAtual = 13;
    this.DataAgendamentodeEntrega = 14;
    this.ETAPrimeiraColeta = 15;
    this.DataCarregamento = 16;
    this.PrevisaoEntrega = 17;
}

EnumMonitoramentoEventoDataHelper.prototype = {
    obterDescricao: function (evento) {
        switch (evento) {
            case this.Padrao: return "Padrão";
            case this.DataAtual: return "Data atual";
            case this.PosicaoVeiculo: return "Posição do veículo";
            case this.EntradaCliente: return "Entrada no cliente";
            case this.SaidaCliente: return "Saída do cliente";
            case this.JanelaCarregamento: return "Janela de carregamento";
            case this.JanelaDescarregamento: return "Janela de descarregamento";
            case this.EntregaPedidoPrevista: return "Entrega do pedido prevista";
            case this.InicioEntregaPrevista: return "Início da entrega prevista";
            case this.FinalEntregaPrevista: return "Fim da entrega prevista";
            case this.InicioEntregaReprogramada: return "ETA entrega";
            case this.FinalEntregaReprogramada: return "Fim da entrega reprogramada";
            case this.InicioEntregaRealizada: return "Início da entrega realizada";
            case this.FinalEntregaRealizada: return "Fim da entrega realizada";
            case this.DataAgendamentodeEntrega: return "Data agendamento de entrega";
            case this.ETAPrimeiraColeta: return "ETA da Primeira Coleta";
            case this.DataCarregamento: return "Data de Carregamento";
            case this.PrevisaoEntrega: return "Previsão de Entrega (inicial)";
            default: return "";
        }
    },

    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Padrao), value: this.Padrao },
            { text: this.obterDescricao(this.DataAtual), value: this.DataAtual },
            { text: this.obterDescricao(this.PosicaoVeiculo), value: this.PosicaoVeiculo },
            { text: this.obterDescricao(this.EntradaCliente), value: this.EntradaCliente },
            { text: this.obterDescricao(this.SaidaCliente), value: this.SaidaCliente },
            { text: this.obterDescricao(this.JanelaCarregamento), value: this.JanelaCarregamento },
            { text: this.obterDescricao(this.JanelaDescarregamento), value: this.JanelaDescarregamento },
            { text: this.obterDescricao(this.EntregaPedidoPrevista), value: this.EntregaPedidoPrevista },
            { text: this.obterDescricao(this.InicioEntregaPrevista), value: this.InicioEntregaPrevista },
            { text: this.obterDescricao(this.FinalEntregaPrevista), value: this.FinalEntregaPrevista },
            { text: this.obterDescricao(this.InicioEntregaReprogramada), value: this.InicioEntregaReprogramada },
            { text: this.obterDescricao(this.FinalEntregaReprogramada), value: this.FinalEntregaReprogramada },
            { text: this.obterDescricao(this.InicioEntregaRealizada), value: this.InicioEntregaRealizada },
            { text: this.obterDescricao(this.FinalEntregaRealizada), value: this.FinalEntregaRealizada },
            { text: this.obterDescricao(this.DataAgendamentodeEntrega), value: this.DataAgendamentodeEntrega },
            { text: this.obterDescricao(this.ETAPrimeiraColeta), value: this.ETAPrimeiraColeta },
            { text: this.obterDescricao(this.DataCarregamento), value: this.DataCarregamento },
            { text: this.obterDescricao(this.PrevisaoEntrega), value: this.PrevisaoEntrega }
        ];
    },

    obterOpcoesFiltradasDataBase: function () {
        return [
            { text: this.obterDescricao(this.DataAgendamentodeEntrega), value: this.DataAgendamentodeEntrega },
            { text: this.obterDescricao(this.PrevisaoEntrega), value: this.PrevisaoEntrega }
        ];
    },

    obterOpcoesFiltradasDataReferencia: function () {
        return [
            { text: this.obterDescricao(this.InicioEntregaReprogramada), value: this.InicioEntregaReprogramada },
        ];
    },

    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesFluxoDePatio: function () {
        return [
            { text: this.obterDescricao(this.EntradaCliente), value: this.EntradaCliente },
            { text: this.obterDescricao(this.SaidaCliente), value: this.SaidaCliente }
        ]
    }
};

var EnumMonitoramentoEventoData = Object.freeze(new EnumMonitoramentoEventoDataHelper());