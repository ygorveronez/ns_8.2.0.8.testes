namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFrete
    {
        CIF = 1,
        FOB = 2,
    }

    public static class TipoFreteHelper
    {
        public static string ObterDescricao(this TipoFrete status)
        {
            switch (status)
            {
                case TipoFrete.CIF: return "CIF";
                case TipoFrete.FOB: return "FOB";
                default: return string.Empty;
            }
        }
    }
}
