using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoItemParametroBaseCalculoTabelaFrete
    {
        Ativo = 0,
        AguardandoRetornoIntegracao = 1,
        ProblemaIntegracao = 2,
        AguardandoIntegracao = 3,
        Aprovacao = 4
    }

    public static class SituacaoItemParametroBaseCalculoTabelaFreteHelper
    {
        public static bool IsAguardandoIntegracao(this SituacaoItemParametroBaseCalculoTabelaFrete situacao)
        {
            return ObterSituacoesAguardandoIntegracao().Contains(situacao);
        }

        public static List<SituacaoItemParametroBaseCalculoTabelaFrete> ObterSituacoesAguardandoIntegracao()
        {
            return new List<SituacaoItemParametroBaseCalculoTabelaFrete>()
            {
                SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoIntegracao,
                SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoRetornoIntegracao,
                SituacaoItemParametroBaseCalculoTabelaFrete.ProblemaIntegracao
            };
        }

        public static List<SituacaoItemParametroBaseCalculoTabelaFrete> ObterSituacoesIntegrado()
        {
            return new List<SituacaoItemParametroBaseCalculoTabelaFrete>()
            {
                SituacaoItemParametroBaseCalculoTabelaFrete.Aprovacao,
                SituacaoItemParametroBaseCalculoTabelaFrete.Ativo
            };
        }
    }
}
