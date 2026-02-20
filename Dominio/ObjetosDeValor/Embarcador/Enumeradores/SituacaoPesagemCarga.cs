using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPesagemCarga
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoPesagemCargaHelper
    {
        public static bool IsPermiteAvancarEtapa(this SituacaoPesagemCarga situacao)
        {
            return ObterSituacoesPermitemAvancarEtapa().Contains(situacao);
        }

        public static string ObterCorIcone(this SituacaoPesagemCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoPesagemCarga.AguardandoAprovacao:
                    return "#edd500";

                case SituacaoPesagemCarga.Aprovada:
                    return "#269917";

                case SituacaoPesagemCarga.Reprovada:
                case SituacaoPesagemCarga.SemRegraAprovacao:
                    return "#DB3F27";

                default:
                    return "#555555";
            }
        }

        public static string ObterDescricao(this SituacaoPesagemCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoPesagemCarga.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoPesagemCarga.Aprovada: return "Aprovada";
                case SituacaoPesagemCarga.NaoInformada: return "Não Informada";
                case SituacaoPesagemCarga.Reprovada: return "Reprovada";
                case SituacaoPesagemCarga.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }

        public static List<SituacaoPesagemCarga> ObterSituacoesPermitemAvancarEtapa()
        {
            return new List<SituacaoPesagemCarga>()
            {
                SituacaoPesagemCarga.Aprovada,
                SituacaoPesagemCarga.NaoInformada,
                SituacaoPesagemCarga.Reprovada
            };
        }
    }
}
