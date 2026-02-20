namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTransferenciaPallet
    {
        Todas = 0,
        AguardandoEnvio = 1,
        EnvioCancelado = 2,
        AguardandoAprovacao = 3,
        SemRegraAprovacao = 4,
        AprovacaoRejeitada = 5,
        AguardandoRecebimento = 6,
        Finalizada = 7
    }

    public static class SituacaoTransferenciaPalletHelper
    {
        public static string ObterDescricao(this SituacaoTransferenciaPallet situacaoTransferenciaPallet)
        {
            switch (situacaoTransferenciaPallet)
            {
                case SituacaoTransferenciaPallet.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoTransferenciaPallet.AguardandoEnvio: return "Aguardando Envio";
                case SituacaoTransferenciaPallet.AguardandoRecebimento: return "Aguardando Recebimento";
                case SituacaoTransferenciaPallet.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case SituacaoTransferenciaPallet.EnvioCancelado: return "Envio Cancelado";
                case SituacaoTransferenciaPallet.Finalizada: return "Finalizada";
                case SituacaoTransferenciaPallet.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return "Todas";
            }
        }
    }
}
