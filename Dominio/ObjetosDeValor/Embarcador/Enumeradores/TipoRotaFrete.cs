namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRotaFrete
    {
        Ida = 0,
        IdaVolta = 1,
        Volta = 2
    }

    public static class TipoRotaFreteHelper
    {
        public static string ObterDescricao(this TipoRotaFrete tipoRotaFrete)
        {
            switch (tipoRotaFrete)
            {
                case TipoRotaFrete.Ida: return "Ida";
                case TipoRotaFrete.IdaVolta: return "Ida e Volta";
                case TipoRotaFrete.Volta: return "Volta";
                default: return string.Empty;
            }
        }
    }
}
