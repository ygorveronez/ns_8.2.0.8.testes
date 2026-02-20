namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EventoColetaEntrega
    {
        Todos = 0,
        Confirma = 1,
        ChegadaNoAlvo = 2,
        IniciaViagem = 3,
        EventosViagem = 4,
        PedidoGerado = 5,
        PedidoFaturado = 6,
        FaturamentoCancelado = 7,
        PedidoCancelado = 8,
        PedidoEmSeparacao = 9,
        RecalculoPrevisao = 10,
        UltimaConfirmacao = 11,
        UltimaChegadaNoAlvo = 12,
        CalculoPrevisao = 13,
        FimViagem = 14,
        Intercorrencia = 15,
        AgendamentoEntrega = 16,
        ReagendamentoEntrega = 17,
        ContatoCliente = 18,
        AlteracaoDataAgendamentoEntregaTransportador = 19,
        AtingirData = 20,
        RejeicaoEntrega = 21,
        EstouIndo = 22
    }

    public static class EventoColetaEntregaHelper
    {
        public static string ObterDescricao(this EventoColetaEntrega eventoColetaEntrega)
        {
            switch (eventoColetaEntrega)
            {
                case EventoColetaEntrega.Todos: return "Todos";
                case EventoColetaEntrega.Confirma: return "Confirmada";
                case EventoColetaEntrega.ChegadaNoAlvo: return "Chegada em Alvo";
                case EventoColetaEntrega.IniciaViagem: return "Inicia Viagem";
                case EventoColetaEntrega.EventosViagem: return "Eventos de Viagem";
                case EventoColetaEntrega.PedidoGerado: return "Pedido Gerado";
                case EventoColetaEntrega.PedidoFaturado: return "Pedido Faturado";
                case EventoColetaEntrega.FaturamentoCancelado: return "Faturamento Cancelado";
                case EventoColetaEntrega.PedidoCancelado: return "Pedido Cancelado";
                case EventoColetaEntrega.PedidoEmSeparacao: return "Pedido em Separação";
                case EventoColetaEntrega.RecalculoPrevisao: return "Recalculo previsão Entrega";
                case EventoColetaEntrega.CalculoPrevisao: return "Calculo previsão Entrega";
                case EventoColetaEntrega.UltimaConfirmacao: return "Ultima Confirmação";
                case EventoColetaEntrega.UltimaChegadaNoAlvo: return "Ultima Chegada no Alvo";
                case EventoColetaEntrega.FimViagem: return "Fim Viagem";
                case EventoColetaEntrega.Intercorrencia: return "Intercorrência";
                case EventoColetaEntrega.AgendamentoEntrega: return "Agendamento da Entrega";
                case EventoColetaEntrega.ReagendamentoEntrega: return "Reagendamento da Entrega";
                case EventoColetaEntrega.ContatoCliente: return "Contato com Cliente";
                case EventoColetaEntrega.AlteracaoDataAgendamentoEntregaTransportador: return "Alteração de Data de Agendamento de Entrega do Transportador";
                case EventoColetaEntrega.AtingirData: return "Atingir Data";
                case EventoColetaEntrega.RejeicaoEntrega: return "Rejeição Entrega";
                case EventoColetaEntrega.EstouIndo: return "Estou Indo";
                default: return "";
            }
        }
    }
}
