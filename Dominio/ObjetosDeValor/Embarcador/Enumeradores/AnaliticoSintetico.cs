namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AnaliticoSintetico
    {
        Analitico = 1,
        Sintetico = 2
    }

    public static class AnaliticoSinteticoHelper
    {
        public static string ObterDescricao(this AnaliticoSintetico analiticoSintetico)
        {
            switch (analiticoSintetico)
            {
                case AnaliticoSintetico.Analitico: return Localization.Resources.Enumeradores.AnaliticoSintetico.Analitico;
                case AnaliticoSintetico.Sintetico: return Localization.Resources.Enumeradores.AnaliticoSintetico.Sintetico;
                default: return string.Empty;
            }
        }
    }
}
