namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusBalanca
    {
        Todos = 0,
        AgIntegracao = 1,
        TicketCriado = 2,
        FalhaIntegracao = 3,
        TicketBloqueado = 4,
        TicketDesbloqueado = 5,
        Encerrado = 6
    }

    public static class StatusBalancaHelper
    {
        public static string ObterDescricao(this StatusBalanca statusBalanca)
        {
            switch (statusBalanca)
            {
                case StatusBalanca.AgIntegracao: return "Ag. Integração";
                case StatusBalanca.TicketCriado: return "Ticket Criado";
                case StatusBalanca.FalhaIntegracao: return "Falha na Integração";
                case StatusBalanca.TicketBloqueado: return "Ticket Bloqueado";
                case StatusBalanca.TicketDesbloqueado: return "Ticket Desbloqueado";
                case StatusBalanca.Encerrado: return "Encerrado";
                default: return string.Empty;
            }
        }
    }
}
