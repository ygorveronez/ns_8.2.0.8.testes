using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCheckin
    {
        SemConfirmacao = 0,
        Confirmado = 1,
        AguardandoAprovacao = 2,
        SemRegraAprovacao = 3,
        RecusaReprovada = 4,
        RecusaAprovada = 5
    }

    public static class SituacaoCheckinHelper
    {
        public static string ObterDescricao(this SituacaoCheckin situacao)
        {
            switch (situacao)
            {
                case SituacaoCheckin.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoCheckin.Confirmado: return "Confirmado";
                case SituacaoCheckin.RecusaAprovada: return "Recusa Aprovada";
                case SituacaoCheckin.RecusaReprovada: return "Recusa Reprovada";
                case SituacaoCheckin.SemConfirmacao: return "Sem Confirmação";
                case SituacaoCheckin.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoCheckin> ObterSituacoesAprovacao()
        {
            return new List<SituacaoCheckin>()
            {
                SituacaoCheckin.AguardandoAprovacao,
                SituacaoCheckin.RecusaAprovada,
                SituacaoCheckin.RecusaReprovada,
                SituacaoCheckin.SemRegraAprovacao
            };
        }

        public static List<SituacaoCheckin> ObterSituacoesPendentes()
        {
            return new List<SituacaoCheckin>()
            {
                SituacaoCheckin.SemConfirmacao,
                SituacaoCheckin.AguardandoAprovacao,
                SituacaoCheckin.RecusaReprovada,
                SituacaoCheckin.SemRegraAprovacao
            };
        }

        public static List<SituacaoCheckin> ObterSituacoesNaoLiberadasPreCheckin()
        {
            return new List<SituacaoCheckin>()
            {
                SituacaoCheckin.AguardandoAprovacao,
                SituacaoCheckin.RecusaReprovada,
                SituacaoCheckin.SemConfirmacao,
            };
        }

        public static List<SituacaoCheckin> ObterSituacoesLiberadasPreCheckin()
        {
            return new List<SituacaoCheckin>()
            {
                SituacaoCheckin.Confirmado,
                SituacaoCheckin.RecusaReprovada,
                SituacaoCheckin.SemRegraAprovacao,
            };
        }
    }
}