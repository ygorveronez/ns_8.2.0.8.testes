namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusViagemControleEntrega
    {
        Iniciada = 1,
        Finalizada = 2,
        NaoIniciada = 3,
        NaoFinalizada = 4,
        EmAndamento = 5
    }

    public static class StatusViagemControleEntregaHelper
    {
        public static string ObterDescricao(this StatusViagemControleEntrega status)
        {
            switch (status)
            {
                case StatusViagemControleEntrega.Iniciada: return "Iniciada";
                case StatusViagemControleEntrega.Finalizada: return "Finalizada";
                case StatusViagemControleEntrega.NaoIniciada: return "Não Iniciada";
                case StatusViagemControleEntrega.NaoFinalizada: return "Não Finalizada";
                case StatusViagemControleEntrega.EmAndamento: return "Em Andamento";

                default: return string.Empty;
            }
        }
    }
}
