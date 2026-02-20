namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoPesquisaTitulo
    {
        Fatura = 0,
        Negociacao = 1,
        ContratoFrete = 2,
        DocumentoEntrada = 3,
        NotaFiscal = 4,
        ContratoFinanciamento = 5,
        Outros = 9
    }

    public static class TipoDocumentoPesquisaTituloHelper
    {
        public static string ObterDescricao(this TipoDocumentoPesquisaTitulo? tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumentoPesquisaTitulo.Fatura: return "Fatura";
                case TipoDocumentoPesquisaTitulo.Negociacao: return "Negociação";
                case TipoDocumentoPesquisaTitulo.ContratoFrete: return "Contrato de Frete";
                case TipoDocumentoPesquisaTitulo.DocumentoEntrada: return "Documento de Entrada";
                case TipoDocumentoPesquisaTitulo.NotaFiscal: return "Nota Fiscal";
                case TipoDocumentoPesquisaTitulo.ContratoFinanciamento: return "Contrato Financiamento";
                case TipoDocumentoPesquisaTitulo.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }
}
