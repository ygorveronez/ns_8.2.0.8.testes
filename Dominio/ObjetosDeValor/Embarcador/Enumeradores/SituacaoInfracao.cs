namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoInfracao
    {
        AguardandoProcessamento = 1,
        Cancelada = 2,
        AguardandoAprovacao = 3,
        SemRegraAprovacao = 4,
        AprovacaoRejeitada = 5,
        Finalizada = 6,
        AguardandoConfirmacaoIntegracao = 7
    }

    public static class SituacaoInfracaoHelper
    {
        public static string ObterDescricao(this SituacaoInfracao situacao)
        {
            switch (situacao)
            {
                case SituacaoInfracao.AguardandoProcessamento: return "Aguardando Processamento";
                case SituacaoInfracao.Cancelada: return "Cancelada";
                case SituacaoInfracao.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoInfracao.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoInfracao.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case SituacaoInfracao.Finalizada: return "Finalizada";
                case SituacaoInfracao.AguardandoConfirmacaoIntegracao: return "Integração Aguardando Confirmação";
                default: return string.Empty;
            }
        }
    }
}
