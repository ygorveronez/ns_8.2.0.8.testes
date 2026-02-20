namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoConfiguracaoNCPendente
    {
        Resumo = 1,
        Individual = 2
    }

    public static class TipoConfiguracaoNCPendenteHelper
    {
        public static string ObterDescricao(this TipoConfiguracaoNCPendente config)
        {
            switch (config)
            {
                case TipoConfiguracaoNCPendente.Resumo: return "Resumo";
                case TipoConfiguracaoNCPendente.Individual: return "Individual";
                default: return string.Empty;
            }
        }
    }
}
