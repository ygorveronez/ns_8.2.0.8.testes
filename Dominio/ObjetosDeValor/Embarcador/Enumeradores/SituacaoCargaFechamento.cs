namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaFechamento
    {
        AgRateio = 0,
        AgCalculoFrete = 1,
        ProblemaCalculoFrete = 2,
        Finalizado = 3
    }

    public static class SituacaoCargaFechamentoHelper
    {
        public static string ObterDescricao(this SituacaoCargaFechamento situacaoFechamentoCarga)
        {
            switch (situacaoFechamentoCarga)
            {
                case SituacaoCargaFechamento.AgRateio: return "Ag. Rateio";
                case SituacaoCargaFechamento.AgCalculoFrete: return "Ag. Calculo de Frete";
                case SituacaoCargaFechamento.ProblemaCalculoFrete: return "Problemas Calculo Frete";
                case SituacaoCargaFechamento.Finalizado: return "Finalizado";
                default: return "";
            }
        }

    }
}
