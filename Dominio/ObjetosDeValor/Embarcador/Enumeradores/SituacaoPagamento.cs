namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPagamento
    {
        Todos = -1,
        EmFechamento = 1,
        PendenciaFechamento = 2,
        AguardandoIntegracao = 3,
        EmIntegracao = 4,
        FalhaIntegracao = 5,
        Finalizado = 6,
        AguardandoAprovacao = 7,
        Reprovado = 8,
        SemRegraAprovacao = 9,
        Cancelado = 10
    }

    public static class SituacaoPagamentoHelper
    {
        public static string ObterDescricao(this SituacaoPagamento situacao)
        {
            switch (situacao)
            {
                case SituacaoPagamento.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoPagamento.AguardandoIntegracao: return "Aguardando Integração";
                case SituacaoPagamento.Reprovado: return "Aprovação Rejeitada";
                case SituacaoPagamento.EmFechamento: return "Em Fechamento";
                case SituacaoPagamento.EmIntegracao: return "Em Integração";
                case SituacaoPagamento.FalhaIntegracao: return "Falha na integração";
                case SituacaoPagamento.Finalizado: return "Finalizado";
                case SituacaoPagamento.PendenciaFechamento: return "Pendência no Fechamento";
                case SituacaoPagamento.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoPagamento.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }

        public static bool PermitirCancelarOuReprocessarPagamento(this SituacaoPagamento situacao)
        {
            return (
                situacao == SituacaoPagamento.EmFechamento ||
                situacao == SituacaoPagamento.PendenciaFechamento ||
                situacao == SituacaoPagamento.AguardandoAprovacao ||
                situacao == SituacaoPagamento.Reprovado ||
                situacao == SituacaoPagamento.SemRegraAprovacao
            );
        }
    }
}
