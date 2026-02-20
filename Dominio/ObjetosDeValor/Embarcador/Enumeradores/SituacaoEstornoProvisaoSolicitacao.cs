using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaCancelamentoSolicitacao
    {
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoCargaCancelamentoSolicitacaoHelper
    {

        public static string ObterCorIcone(this SituacaoCargaCancelamentoSolicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao:
                    return "#edd500";

                case SituacaoCargaCancelamentoSolicitacao.Aprovada:
                    return "#269917";

                case SituacaoCargaCancelamentoSolicitacao.Reprovada:
                case SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao:
                    return "#DB3F27";

                default:
                    return "#555555";
            }
        }

        public static string ObterDescricao(this SituacaoCargaCancelamentoSolicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoCargaCancelamentoSolicitacao.Aprovada: return "Aprovada";
                case SituacaoCargaCancelamentoSolicitacao.Reprovada: return "Reprovada";
                case SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoCargaCancelamentoSolicitacao> ObterSituacoesPendentes()
        {
            return new List<SituacaoCargaCancelamentoSolicitacao>()
            {
                SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao,
                SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao
            };
        }
    }
}
