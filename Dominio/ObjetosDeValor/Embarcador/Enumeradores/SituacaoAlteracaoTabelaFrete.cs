using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAlteracaoTabelaFrete
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoAlteracaoTabelaFreteHelper
    {
        public static bool IsAlteracaoTabelaFreteClienteLiberada(this SituacaoAlteracaoTabelaFrete situacao)
        {
            return (situacao == SituacaoAlteracaoTabelaFrete.Aprovada) || (situacao == SituacaoAlteracaoTabelaFrete.NaoInformada);
        }

        public static List<SituacaoAlteracaoTabelaFrete> ObterSituacoesAlteracaoPendente()
        {
            return new List<SituacaoAlteracaoTabelaFrete>()
            {
                SituacaoAlteracaoTabelaFrete.AguardandoAprovacao,
                SituacaoAlteracaoTabelaFrete.SemRegraAprovacao
            };
        }

        public static string ObterDescricaoPorTabelaFrete(this SituacaoAlteracaoTabelaFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoTabelaFrete.AguardandoAprovacao: return "Valores Aguardando Aprovação";
                case SituacaoAlteracaoTabelaFrete.Aprovada: return "Valores Aprovados";
                case SituacaoAlteracaoTabelaFrete.NaoInformada: return "Valores Aprovados";
                case SituacaoAlteracaoTabelaFrete.Reprovada: return "Valores Reprovados";
                case SituacaoAlteracaoTabelaFrete.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoPorTabelaFreteCliente(this SituacaoAlteracaoTabelaFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoTabelaFrete.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAlteracaoTabelaFrete.Aprovada: return "Aprovada";
                case SituacaoAlteracaoTabelaFrete.NaoInformada: return "Aprovada";
                case SituacaoAlteracaoTabelaFrete.Reprovada: return "Reprovada";
                case SituacaoAlteracaoTabelaFrete.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
