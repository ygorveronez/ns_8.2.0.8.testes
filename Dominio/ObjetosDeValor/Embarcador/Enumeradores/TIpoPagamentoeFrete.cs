namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPagamentoeFrete
    {
        TransferenciaBancaria = 1,
        eFrete = 2,
        Parceiro = 3,
        Outros = 4
    }

    public static class TipoPagamentoeFreteHelper
    {
        public static string ObterDescricao(this TipoPagamentoeFrete tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case TipoPagamentoeFrete.TransferenciaBancaria: return "Transferência Bancária";
                case TipoPagamentoeFrete.eFrete: return "eFrete";
                case TipoPagamentoeFrete.Parceiro: return "Parceiro";
                case TipoPagamentoeFrete.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }
}
