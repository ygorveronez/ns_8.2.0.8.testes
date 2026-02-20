namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FinalidadePagamentoEletronico
    {
        CreditoContaCorrente = 1
    }

    public static class FinalidadePagamentoEletronicoHelper
    {
        public static string ObterDescricao(this FinalidadePagamentoEletronico finalidadePagamento)
        {
            switch (finalidadePagamento)
            {
                case FinalidadePagamentoEletronico.CreditoContaCorrente: return "01 - Conta corrente individual";
                default: return string.Empty;
            }
        }
    }
}
