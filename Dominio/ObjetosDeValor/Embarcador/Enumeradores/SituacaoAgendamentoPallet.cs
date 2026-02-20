namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAgendamentoPallet
    {
        Agendamento = 1,
        Acompanhamento = 2,
        Finalizado = 3,
        Cancelado = 4
    }

    public static class SituacaoAgendamentoPalletHelper
    {
        public static string ObterDescricao(this SituacaoAgendamentoPallet situacao)
        {
            return situacao switch
            {
                SituacaoAgendamentoPallet.Agendamento => "Agendamento",
                SituacaoAgendamentoPallet.Acompanhamento => "Acompanhamento",
                SituacaoAgendamentoPallet.Finalizado => "Concluído",
                SituacaoAgendamentoPallet.Cancelado => "Cancelado",
                _ => string.Empty
            };
        }
    }
}
