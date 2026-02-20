using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCarregamentoSolicitacao
    {
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoCarregamentoSolicitacaoHelper
    {

        public static string ObterCorIcone(this SituacaoCarregamentoSolicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoCarregamentoSolicitacao.AguardandoAprovacao:
                    return "#edd500";

                case SituacaoCarregamentoSolicitacao.Aprovada:
                    return "#269917";

                case SituacaoCarregamentoSolicitacao.Reprovada:
                case SituacaoCarregamentoSolicitacao.SemRegraAprovacao:
                    return "#DB3F27";

                default:
                    return "#555555";
            }
        }

        public static string ObterDescricao(this SituacaoCarregamentoSolicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoCarregamentoSolicitacao.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoCarregamentoSolicitacao.Aprovada: return "Aprovada";
                case SituacaoCarregamentoSolicitacao.Reprovada: return "Reprovada";
                case SituacaoCarregamentoSolicitacao.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoCarregamentoSolicitacao> ObterSituacoesPendentes()
        {
            return new List<SituacaoCarregamentoSolicitacao>()
            {
                SituacaoCarregamentoSolicitacao.AguardandoAprovacao,
                SituacaoCarregamentoSolicitacao.SemRegraAprovacao
            };
        }
    }
}
