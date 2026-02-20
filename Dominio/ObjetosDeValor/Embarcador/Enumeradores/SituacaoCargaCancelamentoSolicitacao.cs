using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEstornoProvisaoSolicitacao
    {
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoEstornoProvisaoSolicitacaoHelper
    {

        public static string ObterCorIcone(this SituacaoEstornoProvisaoSolicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoEstornoProvisaoSolicitacao.AguardandoAprovacao:
                    return "#edd500";

                case SituacaoEstornoProvisaoSolicitacao.Aprovada:
                    return "#269917";

                case SituacaoEstornoProvisaoSolicitacao.Reprovada:
                case SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao:
                    return "#DB3F27";

                default:
                    return "#555555";
            }
        }

        public static string ObterDescricao(this SituacaoEstornoProvisaoSolicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoEstornoProvisaoSolicitacao.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoEstornoProvisaoSolicitacao.Aprovada: return "Aprovado";
                case SituacaoEstornoProvisaoSolicitacao.Reprovada: return "Reprovada";
                case SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoEstornoProvisaoSolicitacao> ObterSituacoesPendentes()
        {
            return new List<SituacaoEstornoProvisaoSolicitacao>()
            {
                SituacaoEstornoProvisaoSolicitacao.AguardandoAprovacao,
                SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao
            };
        }
    }
}
