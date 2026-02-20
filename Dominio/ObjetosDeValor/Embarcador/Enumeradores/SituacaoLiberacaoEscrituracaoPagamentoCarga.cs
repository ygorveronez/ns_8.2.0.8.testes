namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLiberacaoEscrituracaoPagamentoCarga
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoLiberacaoEscrituracaoPagamentoCargaHelper
    {
        public static bool IsLiberada(this SituacaoLiberacaoEscrituracaoPagamentoCarga situacao)
        {
            return (
                (situacao == SituacaoLiberacaoEscrituracaoPagamentoCarga.Aprovada) ||
                (situacao == SituacaoLiberacaoEscrituracaoPagamentoCarga.NaoInformada) ||
                (situacao == SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao)
            );
        }

        public static string ObterDescricao(this SituacaoLiberacaoEscrituracaoPagamentoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoLiberacaoEscrituracaoPagamentoCarga.Aprovada: return "Aprovada";
                case SituacaoLiberacaoEscrituracaoPagamentoCarga.NaoInformada: return "Não Informada";
                case SituacaoLiberacaoEscrituracaoPagamentoCarga.Reprovada: return "Reprovada";
                case SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
