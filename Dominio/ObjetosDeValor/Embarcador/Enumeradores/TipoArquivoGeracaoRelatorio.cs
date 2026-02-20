namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoArquivoGeracaoRelatorio
    {
        Excel = 1,
        Pdf = 2,
        PdfEExcel = 3
    }

    public static class TipoArquivoGeracaoRelatorioHelper
    {
        public static string ObterDescricao(this TipoArquivoGeracaoRelatorio tipoArquivoGeracaoRelatorio)
        {
            switch (tipoArquivoGeracaoRelatorio)
            {
                case TipoArquivoGeracaoRelatorio.Excel: return "Excel";
                case TipoArquivoGeracaoRelatorio.Pdf: return "PDF";
                case TipoArquivoGeracaoRelatorio.PdfEExcel: return "PDF e Excel";
                default: return string.Empty;
            }
        }
    }
}
