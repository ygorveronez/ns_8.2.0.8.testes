namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumTipoPagamentoAutorizacaoPagamento
    {
        PagamentoAdiantamento = 1,
        PagamentoSaldo = 2
    }

    public static class EnumTipoPagamentoAutorizacaoPagamentoHelper
    {
        public static string ObterDescricao(this EnumTipoPagamentoAutorizacaoPagamento tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento: return "Pagamento Adiantamento";
                case EnumTipoPagamentoAutorizacaoPagamento.PagamentoSaldo: return "Pagamento Saldo";
                default: return string.Empty;
            }
        }

    }
}
