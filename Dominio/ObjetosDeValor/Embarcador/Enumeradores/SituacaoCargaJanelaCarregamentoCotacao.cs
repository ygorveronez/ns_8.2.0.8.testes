namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaCarregamentoCotacao
    {
        NaoDefinida = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoCargaJanelaCarregamentoCotacaoHelper
    {
        public static bool IsPendenteAprovacao(this SituacaoCargaJanelaCarregamentoCotacao situacao)
        {
            return (situacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) || (situacao == SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao);
        }

        public static string ObterDescricao(this SituacaoCargaJanelaCarregamentoCotacao situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoCargaJanelaCarregamentoCotacao.Aprovada: return "Aprovada";
                case SituacaoCargaJanelaCarregamentoCotacao.NaoDefinida: return "Não Definida";
                case SituacaoCargaJanelaCarregamentoCotacao.Reprovada: return "Reprovada";
                case SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
