var EnumEventoColetaEntregaHelper = function () {
    this.Todos = 0;
    this.Confirma = 1;
    this.ChegadaNoAlvo = 2;
    this.IniciaViagem = 3;
    this.PedidoGerado = 5;
    this.PedidoFaturado = 6;
    this.FaturamentoCancelado = 7;
    this.PedidoCancelado = 8;
    this.PedidoEmSeparacao = 9
    this.RecalculoPrevisao = 10;
    this.UltimaConfirmacao = 11;
    this.UltimaChegadaNoAlvo = 12;
    this.CalculoPrevisao = 13;
    this.FimViagem = 14;
    this.Intercorrencia = 15;
    this.AgendamentoEntrega = 16;
    this.ReagendamentoEntrega = 17;
    this.ContatoCliente = 18;
    this.AlteracaoDataAgendamentoEntregaTransportador = 19;
    this.AtingirData = 20;
    this.RejeicaoEntrega = 21;
    this.EstouIndo = 22;
};

EnumEventoColetaEntregaHelper.prototype = {
    obterOpcoesOcorrenciaEntrega: function () {
        return [
            { text: "Confirmação", value: this.Confirma },
            { text: "Chegada no Alvo", value: this.ChegadaNoAlvo },
            { text: "Viagem iniciada", value: this.IniciaViagem },
            { text: "Recalculo Previsão Entrega", value: this.RecalculoPrevisao },
            { text: "Calculo Previsão Entrega", value: this.CalculoPrevisao },
            { text: "Ultima Confirmação", value: this.UltimaConfirmacao },
            { text: "Ultima Chegada no Alvo", value: this.UltimaChegadaNoAlvo },
            { text: "Fim Viagem", value: this.FimViagem },
            { text: "Intercorrência", value: this.Intercorrencia },
            { text: "Agendamento de Entrega", value: this.AgendamentoEntrega },
            { text: "Reagendamento de Entrega", value: this.ReagendamentoEntrega },
            { text: "Contato com Cliente", value: this.ContatoCliente },
            { text: "Alteração de Data de Agendamento de Entrega do Transportador", value: this.AlteracaoDataAgendamentoEntregaTransportador },
            { text: "Atingimento de Data", value: this.AtingirData },
            { text: "Estou Indo", value: this.EstouIndo },
        ];
    },
    obterOpcoesOcorrenciaPedido: function () {
        return [
            { text: "Pedido Gerado", value: this.PedidoGerado },
            { text: "Pedido em Separação", value: this.PedidoEmSeparacao },
            { text: "Pedido Faturado", value: this.PedidoFaturado },
            { text: "Faturamento Cancelado", value: this.FaturamentoCancelado },
            { text: "Pedido Cancelado", value: this.PedidoCancelado },
            { text: "Inicio de Viagem", value: this.IniciaViagem }
        ];
    },
    obterOpcoesPesquisaEntrega: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Confirmação", value: this.Confirma },
            { text: "Chegada no Alvo", value: this.ChegadaNoAlvo },
            { text: "Viagem iniciada", value: this.IniciaViagem },
            { text: "Recalculo Previsão Entrega", value: this.RecalculoPrevisao },
            { text: "Calculo Previsão Entrega", value: this.CalculoPrevisao },
            { text: "Ultima Confirmação", value: this.UltimaConfirmacao },
            { text: "Ultima Chegada no Alvo", value: this.UltimaChegadaNoAlvo },
            { text: "Fim Viagem", value: this.FimViagem },
            { text: "Intercorrência", value: this.Intercorrencia },
            { text: "Agendamento de Entrega", value: this.AgendamentoEntrega },
            { text: "Reagendamento de Entrega", value: this.ReagendamentoEntrega },
            { text: "Contato com Cliente", value: this.ContatoCliente }
        ];
    },
    obterOpcoesPesquisaPedido: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Pedido Gerado", value: this.PedidoGerado },
            { text: "Pedido em Separação", value: this.PedidoEmSeparacao },
            { text: "Pedido Faturado", value: this.PedidoFaturado },
            { text: "Faturamento Cancelado", value: this.FaturamentoCancelado },
            { text: "Pedido Cancelado", value: this.PedidoCancelado },
            { text: "Inicio de Viagem", value: this.IniciaViagem },
            { text: "Ultima Confirmação", value: this.UltimaConfirmacao },
            { text: "Ultima Chegada no Alvo", value: this.UltimaChegadaNoAlvo }
        ];
    }
}

var EnumEventoColetaEntrega = Object.freeze(new EnumEventoColetaEntregaHelper());