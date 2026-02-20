namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoProvedor
    {
        Nenhum = 0,
        CTe = 1,
        CTeComplementar = 2,
        NFSe = 3
    }

    public static class TipoDocumentoProvedorHelper
    {
        public static string ObterDescricao(this TipoDocumentoProvedor etapa)
        {
            switch (etapa)
            {
                case TipoDocumentoProvedor.Nenhum: return "Nenhum";
                case TipoDocumentoProvedor.CTe: return "CTe";
                case TipoDocumentoProvedor.CTeComplementar: return "CTe Complementar";
                case TipoDocumentoProvedor.NFSe: return "NFSe";
                default: return string.Empty;
            }
        }
    }
}
