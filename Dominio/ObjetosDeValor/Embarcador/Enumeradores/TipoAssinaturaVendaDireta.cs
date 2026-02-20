namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAssinaturaVendaDireta
    {
        Todos = 0,
        Presencial = 1,
        Videoconferencia = 2
    }

    public static class TipoAssinaturaVendaDiretaHelper
    {
        public static string ObterDescricao(this TipoAssinaturaVendaDireta tipoAssinatura)
        {
            switch (tipoAssinatura)
            {
                case TipoAssinaturaVendaDireta.Presencial: return "Presencial";
                case TipoAssinaturaVendaDireta.Videoconferencia: return "Videoconferência";
                default: return "Não informado";
            }
        }
    }
}
