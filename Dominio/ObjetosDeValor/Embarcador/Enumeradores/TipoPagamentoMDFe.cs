namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPagamentoMDFe
    {
        PIX = 1,
        Banco = 2,
        Ipef = 3
    }

    public static class TipoPagamentoMDFeHelper
    {
        public static string ObterDescricao(this TipoPagamentoMDFe tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case TipoPagamentoMDFe.PIX:
                    return "PIX";
                case TipoPagamentoMDFe.Banco:
                    return "Banco";
                case TipoPagamentoMDFe.Ipef:
                    return "Ipef";
                default:
                    return string.Empty;
            }
        }
    }


}
