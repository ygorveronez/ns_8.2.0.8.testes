using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAprovacaoSolicitacaoGas
    {
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }
    
    public static class SituacaoAprovacaoSolicitacaoGasHelper
    {

        public static string ObterCorIcone(this SituacaoAprovacaoSolicitacaoGas situacao)
        {
            switch (situacao)
            {
                case SituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao:
                    return "#edd500";

                case SituacaoAprovacaoSolicitacaoGas.Aprovada:
                    return "#269917";

                case SituacaoAprovacaoSolicitacaoGas.Reprovada:
                case SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao:
                    return "#DB3F27";

                default:
                    return "#555555";
            }
        }

        public static string ObterDescricao(this SituacaoAprovacaoSolicitacaoGas situacao)
        {
            switch (situacao)
            {
                case SituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAprovacaoSolicitacaoGas.Aprovada: return "Aprovada";
                case SituacaoAprovacaoSolicitacaoGas.Reprovada: return "Reprovada";
                case SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoAprovacaoSolicitacaoGas> ObterSituacoesPendentes()
        {
            return new List<SituacaoAprovacaoSolicitacaoGas>()
            {
                SituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao,
                SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao
            };
        }
    }
}
