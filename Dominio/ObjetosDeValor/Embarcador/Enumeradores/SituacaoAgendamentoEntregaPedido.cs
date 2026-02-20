namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAgendamentoEntregaPedido
    {
        AguardandoAgendamento = 1,
        Agendado = 2,
        Finalizado = 3,
        ReagendamentoSolicitado = 4,
        AguardandoRetornoCliente = 5,
        AguardandoReagendamento = 6,
        Reagendado = 7,
        NaoExigeAgendamento = 8
    }
    
    public static class SituacaoAgendamentoEntregaPedidoHelper
    {
        public static string ObterDescricao(this SituacaoAgendamentoEntregaPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoAgendamentoEntregaPedido.AguardandoAgendamento: return "Aguardando Agendamento";
                case SituacaoAgendamentoEntregaPedido.Agendado: return "Agendado";
                case SituacaoAgendamentoEntregaPedido.Finalizado: return "Finalizado";
                case SituacaoAgendamentoEntregaPedido.ReagendamentoSolicitado: return "Reagendamento Solicitado";
                case SituacaoAgendamentoEntregaPedido.AguardandoRetornoCliente: return "Aguardando Retorno Cliente";
                case SituacaoAgendamentoEntregaPedido.AguardandoReagendamento: return "Aguardando Reagendamento";
                case SituacaoAgendamentoEntregaPedido.Reagendado: return "Reagendado";
                case SituacaoAgendamentoEntregaPedido.NaoExigeAgendamento: return "Não Exige Agendamento";
                default: return string.Empty;
            }
        }
    }
}
