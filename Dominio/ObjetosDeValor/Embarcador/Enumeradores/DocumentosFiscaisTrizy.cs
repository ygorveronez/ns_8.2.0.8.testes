namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DocumentosFiscaisTrizy
    {
        Nenhum = 0,
        CTe = 1,
        MDFe = 2,
        NFe = 3,
        CIOT = 4,
        VPO = 5
    }

    public static class DocumentosFiscaisTrizyHelper
    {
        public static string ObterDescricao(this DocumentosFiscaisTrizy DocumentosFiscaisTrizy)
        {
            switch (DocumentosFiscaisTrizy)
            {
                case DocumentosFiscaisTrizy.Nenhum: return "Nenhum";
                case DocumentosFiscaisTrizy.CTe: return "CTe";
                case DocumentosFiscaisTrizy.MDFe: return "MDFe";
                case DocumentosFiscaisTrizy.NFe: return "NFe";
                case DocumentosFiscaisTrizy.CIOT: return "CIOT";
                case DocumentosFiscaisTrizy.VPO: return "VPO";
                default: return string.Empty;
            }
        }
        public static string ObterIDTrizy(this DocumentosFiscaisTrizy DocumentosFiscaisTrizy)
        {
            switch (DocumentosFiscaisTrizy)
            {
                case DocumentosFiscaisTrizy.CTe: return "646cc9c891f83488566544d1";
                case DocumentosFiscaisTrizy.MDFe: return "646cc9c891f83488566544d3";
                case DocumentosFiscaisTrizy.NFe: return "646cc9c891f83488566544db";
                case DocumentosFiscaisTrizy.CIOT: return "64c81d932d000dc7612bea69";
                case DocumentosFiscaisTrizy.VPO: return "646cc9c891f83488566544da";
                default: return string.Empty;
            }
        }
    }
}