namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumento
    {
        NotaFiscal = 1,
        NFe = 55,
        CTe = 57,
        DCe = 100,
        Outros = 99,
    }

    public static class TipoDocumentoHelper
    {
        public static string ObterDescricao(this TipoDocumento tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumento.NFe: return "NF-e";
                case TipoDocumento.CTe: return "CT-e";
                case TipoDocumento.NotaFiscal: return "Nota Fiscal";
                case TipoDocumento.DCe: return "DC-e";
                case TipoDocumento.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }
}
