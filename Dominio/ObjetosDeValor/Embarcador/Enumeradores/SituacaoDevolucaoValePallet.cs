namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDevolucaoValePallet
    {
        Todas = 0,
        AguardandoAprovacao = 1,
        SemRegraAprovacao = 2,
        AprovacaoRejeitada = 3,
        Finalizada = 4
    }

    public static class SituacaoDevolucaoValePalletHelper
    {
        public static string ObterDescricao(this SituacaoDevolucaoValePallet situacaoDevolucaoValePallet)
        {
            switch (situacaoDevolucaoValePallet)
            {
                case SituacaoDevolucaoValePallet.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoDevolucaoValePallet.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case SituacaoDevolucaoValePallet.Finalizada: return "Finalizada";
                case SituacaoDevolucaoValePallet.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return "Todas";
            }
        }
    }
}
