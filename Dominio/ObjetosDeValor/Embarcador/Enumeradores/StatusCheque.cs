namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusCheque
    {
        Cancelado = 1,
        Compensado = 2,
        Pendente = 3,
        Normal = 4,
        Devolvido = 5,
        Depositado = 6,
        SemFundos = 7,
        Reapresentado = 8,
        Repassado = 9,
        Sustado = 10,
        Disponivel = 11
    }

    public static class StatusChequeHelper
    {
        public static string ObterDescricao(this StatusCheque status)
        {
            switch (status)
            {
                case StatusCheque.Cancelado: return "Cancelado";
                case StatusCheque.Compensado: return "Compensado";
                case StatusCheque.Normal: return "Normal";
                case StatusCheque.Pendente: return "Pendente";
                case StatusCheque.Devolvido: return "Devolvido";
                case StatusCheque.Depositado: return "Depositado";
                case StatusCheque.SemFundos: return "Sem Fundos";
                case StatusCheque.Reapresentado: return "Reapresentado";
                case StatusCheque.Repassado: return "Repassado";
                case StatusCheque.Sustado: return "Sustado";
                case StatusCheque.Disponivel: return "Disponível";
                default: return string.Empty;
            }
        }
    }
}
