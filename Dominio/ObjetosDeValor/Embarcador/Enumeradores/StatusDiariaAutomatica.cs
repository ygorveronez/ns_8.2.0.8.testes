namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusDiariaAutomatica
    {
        Nenhum = 0,
        EsperandoNoLocal = 1,
        EmDeslocamento = 2,
        Finalizada = 3,
        Cancelada = 4,
    }

    public static class StatusDiariaAutomaticaHelper
    {
        public static string ObterDescricao(this StatusDiariaAutomatica status)
        {
            switch (status)
            {
                case StatusDiariaAutomatica.EsperandoNoLocal: return "Esperando no local";
                case StatusDiariaAutomatica.EmDeslocamento: return "Em deslocamento";
                case StatusDiariaAutomatica.Finalizada: return "Finalizada";
                case StatusDiariaAutomatica.Cancelada: return "Cancelada";
                default: return string.Empty;
            }
        }
    }
}
