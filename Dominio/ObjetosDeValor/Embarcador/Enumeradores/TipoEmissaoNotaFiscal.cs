namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissaoNotaFiscal
    {
        NaoEletronica = 0,
        Normal = 1,
        ContingenciaFSIA = 2,
        ContingenciaSCAN = 3,
        ContingenciaEPEC = 4,
        ContingenciaFSDA = 5,
        ContingenciaSVCAN = 6,
        ContingenciaSVCRS = 7,
        ContingenciaOffLine = 9
    }

    public static class TipoEmissaoNotaFiscalHelper
    {
        public static string ObterDescricao(this TipoEmissaoNotaFiscal tipoEmissao)
        {
            switch (tipoEmissao)
            {
                case TipoEmissaoNotaFiscal.Normal: return "Emissão normal";
                case TipoEmissaoNotaFiscal.ContingenciaFSIA: return "Contingência FS-IA";
                case TipoEmissaoNotaFiscal.ContingenciaSCAN: return "Contingência SCAN";
                case TipoEmissaoNotaFiscal.ContingenciaEPEC: return "Contingência DPEC";
                case TipoEmissaoNotaFiscal.ContingenciaFSDA: return "Contingência FS-DA";
                case TipoEmissaoNotaFiscal.ContingenciaSVCAN: return "Contingência SVC-AN";
                case TipoEmissaoNotaFiscal.ContingenciaSVCRS: return "Contingência SVC-RS";
                case TipoEmissaoNotaFiscal.ContingenciaOffLine: return "Contingência off-line da NFC-e";
                default: return string.Empty;
            }
        }
    }
}
