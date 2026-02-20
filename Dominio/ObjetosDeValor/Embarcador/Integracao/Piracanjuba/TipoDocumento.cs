namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba
{
    public enum TipoDocumento
    {
        Fim = 0,
        CTe = 1,
        MDFe = 2,
        ValePedagio = 3
    }

    public static class TipoDocumentoHelper
    {
        public static string ObterValorIntegracao(this TipoDocumento tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumento.Fim: return "F";
                case TipoDocumento.CTe: return "C";
                case TipoDocumento.MDFe: return "M";
                case TipoDocumento.ValePedagio: return "V";
                default: return string.Empty;
            }
        }
    }
}
