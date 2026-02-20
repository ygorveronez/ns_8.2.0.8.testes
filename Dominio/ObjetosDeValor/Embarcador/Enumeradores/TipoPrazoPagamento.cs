namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPrazoPagamento
    {
        DataPagamento = 1,
        DataDocumento = 2,
        DataLiberacaoDocumento = 4,
        ApartirAprovacaoFatura = 5,
        ApartirDataGeracaoFatura = 6
    }

    public static class TipoPrazoPagamentoHelper
    {
        public static string ObterDescricao(this TipoPrazoPagamento tipo)
        {
            switch (tipo)
            {
                case TipoPrazoPagamento.DataDocumento: return "A partir da emissão do primeiro documento";
                case TipoPrazoPagamento.DataPagamento: return "A partir da data do pagamento";
                case TipoPrazoPagamento.DataLiberacaoDocumento: return "A partir da data de liberação do documento";
                case TipoPrazoPagamento.ApartirAprovacaoFatura: return "A partir da aprovação da fatura";
                case TipoPrazoPagamento.ApartirDataGeracaoFatura: return "A partir da geração da fatura";
                default: return string.Empty;
            }
        }
    }
}
