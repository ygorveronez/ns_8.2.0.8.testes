namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusAgendaTarefa
    {
        Aberto = 1,
        EmAndamento = 2,
        Cancelado = 3,
        Finalizado = 4
    }

    public static class StatusAgendaTarefaHelper
    {
        public static string ObterDescricao(this StatusAgendaTarefa status)
        {
            switch (status)
            {
                case StatusAgendaTarefa.Aberto: return "Aberto";
                case StatusAgendaTarefa.EmAndamento: return "Em Andamento";
                case StatusAgendaTarefa.Cancelado: return "Cancelado";
                case StatusAgendaTarefa.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
