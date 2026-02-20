namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAgendamentoEntrega
    {
        AguardandoConfirmacao = 1,
        Agendado = 2,
        Finalizado = 3
    }

    public static class SituacaoAgendamentoEntregaHelper
    {
        public static string ObterDescricao(this SituacaoAgendamentoEntrega situacao)
        {
            switch (situacao)
            {
                case SituacaoAgendamentoEntrega.AguardandoConfirmacao: return "Aguardando Confirmação";
                case SituacaoAgendamentoEntrega.Agendado: return "Agendado";
                case SituacaoAgendamentoEntrega.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
