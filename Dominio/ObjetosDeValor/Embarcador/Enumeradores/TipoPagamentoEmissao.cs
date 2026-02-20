namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPagamentoEmissao
    {
        Pago = 0,
        A_Pagar = 1,
        Outros = 2,
        UsarDaNotaFiscal = 99
    }

    public static class TipoPagamentoEmissaoHelper
    {
        public static string ObterDescricao(this TipoPagamentoEmissao tipoPagamentoEmissao)
        {
            switch (tipoPagamentoEmissao)
            {
                case TipoPagamentoEmissao.Pago:
                    return "Pago";
                case TipoPagamentoEmissao.A_Pagar:
                    return "A Pagar";
                case TipoPagamentoEmissao.Outros:
                    return "Outros";
                case TipoPagamentoEmissao.UsarDaNotaFiscal:
                    return "Usar da Nota Fiscal";
                default:
                    return string.Empty;
            }
        }
    }
}
