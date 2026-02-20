namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaAutorizacaoTabelaFrete
    {
        Todas = 9,
        AprovacaoReajuste = 0,
        IntegracaoReajuste = 1
    }

    public static class EtapaAutorizacaoTabelaFreteHelper
    {
        public static string ObterDescricao(this EtapaAutorizacaoTabelaFrete etapaAutorizacao)
        {
            switch (etapaAutorizacao)
            {
                case EtapaAutorizacaoTabelaFrete.AprovacaoReajuste: return "Aprovação do Reajuste";
                case EtapaAutorizacaoTabelaFrete.IntegracaoReajuste: return "Integração do Reajuste";
                default: return string.Empty;
            }
        }
    }
}