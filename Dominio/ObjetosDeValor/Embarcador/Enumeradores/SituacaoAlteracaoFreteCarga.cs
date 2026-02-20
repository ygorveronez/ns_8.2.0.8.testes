using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAlteracaoFreteCarga
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoAlteracaoFreteCargaHelper
    {
        public static bool IsPermiteAvancarEtapa(this SituacaoAlteracaoFreteCarga situacao)
        {
            return ObterSituacoesPermitemAvancarEtapa().Contains(situacao);
        }

        public static string ObterCorIcone(this SituacaoAlteracaoFreteCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoFreteCarga.AguardandoAprovacao:
                    return "#edd500";

                case SituacaoAlteracaoFreteCarga.Aprovada:
                    return "#269917";

                case SituacaoAlteracaoFreteCarga.Reprovada:
                case SituacaoAlteracaoFreteCarga.SemRegraAprovacao:
                    return "#DB3F27";

                default:
                    return "#555555";
            }
        }

        public static string ObterDescricao(this SituacaoAlteracaoFreteCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoFreteCarga.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAlteracaoFreteCarga.Aprovada: return "Aprovada";
                case SituacaoAlteracaoFreteCarga.NaoInformada: return "Não Informada";
                case SituacaoAlteracaoFreteCarga.Reprovada: return "Reprovada";
                case SituacaoAlteracaoFreteCarga.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoAlteracaoFreteCarga> ObterSituacoesPermitemAvancarEtapa()
        {
            return new List<SituacaoAlteracaoFreteCarga>()
            {
                SituacaoAlteracaoFreteCarga.Aprovada,
                SituacaoAlteracaoFreteCarga.NaoInformada,
                SituacaoAlteracaoFreteCarga.Reprovada
            };
        }
    }
}
