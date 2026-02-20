namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoHistoricoAgendamento
    {
        SugestaoDeAgendamento = 1,
        Agendamento = 3,
        SugestaoDeReagendamento = 2,
        Reagendamento = 4
    }

    public static class TipoHistoricoAgendamentoHelper
    {
        public static string ObterDescricao(this TipoHistoricoAgendamento TipoHistoricoAgendamento)
        {
            switch (TipoHistoricoAgendamento)
            {
                case TipoHistoricoAgendamento.SugestaoDeAgendamento: return "Sugerir Data de Entrega";
                case TipoHistoricoAgendamento.Agendamento: return "Agendar Descarga";
                case TipoHistoricoAgendamento.SugestaoDeReagendamento: return "Sugestão de Reagendamento";
                case TipoHistoricoAgendamento.Reagendamento: return "Reagendar";
                default: return string.Empty;
            }
        }
    }
}
