using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoNaoConformidade
    {
        AguardandoTratativa = 1,
        Concluida = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4,
        ConcluidaPorIntegracao = 5,
        AprovadaEmContingencia = 6
    }

    public static class SituacaoNaoConformidadeHelper
    {
        public static bool IsPermitirAjustar(this SituacaoNaoConformidade situacao)
        {
            return (
                (situacao == SituacaoNaoConformidade.AguardandoTratativa) ||
                (situacao == SituacaoNaoConformidade.Reprovada) ||
                (situacao == SituacaoNaoConformidade.SemRegraAprovacao)
            );
        }

        public static string ObterDescricao(this SituacaoNaoConformidade situacao)
        {
            switch (situacao)
            {
                case SituacaoNaoConformidade.AguardandoTratativa: return "Aguardando tratativa";
                case SituacaoNaoConformidade.Concluida: return "Concluída";
                case SituacaoNaoConformidade.Reprovada: return "Reprovada";
                case SituacaoNaoConformidade.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoNaoConformidade.ConcluidaPorIntegracao: return "Concluída por integração";
                case SituacaoNaoConformidade.AprovadaEmContingencia: return "Aprovada em contingência";
                default: return string.Empty;
            }
        }

        public static List<SituacaoNaoConformidade> ObterSituacoesPendentes()
        {
            return new List<SituacaoNaoConformidade>()
            {
                SituacaoNaoConformidade.AguardandoTratativa,
                SituacaoNaoConformidade.Reprovada,
                SituacaoNaoConformidade.SemRegraAprovacao
            };
        }
    }
}
