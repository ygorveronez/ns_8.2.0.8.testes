namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoArquivo
    {
        Zip = 1,
        PDF = 2
    }

    public static class TipoArquivoHelper
    {
        public static string ObterDescricao(this TipoArquivo tipoArquivo)
        {
            switch (tipoArquivo)
            {
                case TipoArquivo.PDF: return "PDF";
                case TipoArquivo.Zip: return "ZIP";
                default: return string.Empty;
            }
        }

        public static string ObterExtensao(this TipoArquivo tipoArquivo)
        {
            switch (tipoArquivo)
            {
                case TipoArquivo.PDF: return "pdf";
                case TipoArquivo.Zip: return "zip";
                default: return string.Empty;
            }
        }

        public static IconesNotificacao ObterIconeNotificacao(this TipoArquivo tipoArquivo)
        {
            switch (tipoArquivo)
            {
                case TipoArquivo.PDF: return IconesNotificacao.pdf;
                case TipoArquivo.Zip: return IconesNotificacao.arquivoCompactado;
                default: return IconesNotificacao.sucesso;
            }
        }
    }
}
