namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAvariaPallet
    {
        Todas = 0,
        AguardandoAprovacao = 1,
        SemRegraAprovacao = 2,
        AprovacaoRejeitada = 3,
        Finalizada = 4
    }

    public static class SituacaoAvariaPalletHelper
    {
        public static string ObterDescricao(this SituacaoAvariaPallet situacaoTransferenciaPallet)
        {
            switch (situacaoTransferenciaPallet)
            {
                case SituacaoAvariaPallet.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAvariaPallet.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case SituacaoAvariaPallet.Finalizada: return "Finalizada";
                case SituacaoAvariaPallet.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return "Todas";
            }
        }
    }
}
