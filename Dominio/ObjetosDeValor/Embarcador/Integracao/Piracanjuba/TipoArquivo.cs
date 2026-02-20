namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba
{
    public enum TipoArquivo
    {
        Erro = 0,
        PDF = 1,
        XML = 2,
        Fim = 3,
        Del = 4
    }

    public static class TipoArquivoHelper
    {
        public static string ObterDescricao(this TipoArquivo tipoArquivo)
        {
            switch (tipoArquivo)
            {
                case TipoArquivo.Erro: return "ERR";
                case TipoArquivo.PDF: return "PDF";
                case TipoArquivo.XML: return "XML";
                case TipoArquivo.Fim: return "FIM";
                case TipoArquivo.Del: return "DEL";
                default: return string.Empty;
            }
        }
    }
}
