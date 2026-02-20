namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoContratoPrestacaoServico
    {
        Aprovado = 1,
        AguardandoAprovacao = 2,
        SemRegraAprovacao = 3,
        AprovacaoRejeitada = 4
    }

    public static class SituacaoContratoPrestacaoServicoHelper
    {
        public static bool IsPermiteAtualizar(this SituacaoContratoPrestacaoServico situacao)
        {
            return (situacao != SituacaoContratoPrestacaoServico.AguardandoAprovacao);
        }

        public static string ObterDescricao(this SituacaoContratoPrestacaoServico situacao)
        {
            switch (situacao)
            {
                case SituacaoContratoPrestacaoServico.Aprovado: return "Aprovado";
                case SituacaoContratoPrestacaoServico.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoContratoPrestacaoServico.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoContratoPrestacaoServico.AprovacaoRejeitada: return "Aprovação Rejeitada";
                default: return string.Empty;
            }
        }
    }
}
