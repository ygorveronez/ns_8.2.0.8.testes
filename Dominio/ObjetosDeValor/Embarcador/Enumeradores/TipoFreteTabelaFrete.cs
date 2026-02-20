namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFreteTabelaFrete
    {
        NaoInformado = 0,
        Proprio = 1,
        Spot = 2,
        Terceiro = 3
    }

    public static class TipoFreteTabelaFreteHelper
    {
        public static string ObterDescricao(this TipoFreteTabelaFrete tipoFrete)
        {
            switch (tipoFrete)
            {
                case TipoFreteTabelaFrete.NaoInformado: return "Não Informado";
                case TipoFreteTabelaFrete.Proprio: return "Próprio";
                case TipoFreteTabelaFrete.Spot: return "Spot";
                case TipoFreteTabelaFrete.Terceiro: return "Terceiro";
                default: return string.Empty;
            }
        }
    }
}
