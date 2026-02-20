namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoServico
    {
        NotaFiscal = 1,
        CupomFiscal = 2,
        Contrato = 3,
        Recibo = 4,
        NotaFiscalConjugada = 5,
        NotaFiscalServicoEletronica = 7,
        CupomFiscalConjugado = 8
    }

    public static class TipoDocumentoServicoHelper
    {
        public static string ObterDescricao(this TipoDocumentoServico tipoDocumentoServico)
        {
            switch (tipoDocumentoServico)
            {
                case TipoDocumentoServico.NotaFiscal: return "1 - Nota Fiscal";
                case TipoDocumentoServico.CupomFiscal: return "2 - Cupom Fiscal";
                case TipoDocumentoServico.Contrato: return "3 - Contrato";
                case TipoDocumentoServico.Recibo: return "4 - Recibo";
                case TipoDocumentoServico.NotaFiscalConjugada: return "5 - Nota Fiscal Conjugada";
                case TipoDocumentoServico.NotaFiscalServicoEletronica: return "7 - Nota Fiscal de Serviço Eletrônica";
                case TipoDocumentoServico.CupomFiscalConjugado: return "8 - Cupom Fiscal Conjugado";
                default: return string.Empty;
            }
        }
    }
}
