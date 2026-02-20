namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoViagemComponenteTabelaFrete
    {
        Propria = 0,
        Terceiros = 1
    }

    public static class TipoViagemComponenteTabelaFreteHelper
    {
        public static string ObterDescricao(this TipoViagemComponenteTabelaFrete tipoViagemComponenteTabelaFrete)
        {
            switch (tipoViagemComponenteTabelaFrete)
            {
                case TipoViagemComponenteTabelaFrete.Propria: return "Própria";
                case TipoViagemComponenteTabelaFrete.Terceiros: return "Terceiros";
                default: return "";
            }
        }
    }
}
