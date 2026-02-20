namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCancelamentoPagamento
    {
        EmCancelamento = 1,
        PendenciaCancelamento = 2,
        AgIntegracao = 3,
        EmIntegracao = 4,
        FalhaIntegracao = 5,
        Cancelado = 6
    }

    public static class SituacaoCancelamentoPagamentoHelper
    {
        public static string ObterDescricao(this SituacaoCancelamentoPagamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCancelamentoPagamento.EmCancelamento: return "Em Cancelamento";
                case SituacaoCancelamentoPagamento.PendenciaCancelamento: return "Pendência de Cancelamento";
                case SituacaoCancelamentoPagamento.AgIntegracao: return "Aguardando Intergração";
                case SituacaoCancelamentoPagamento.EmIntegracao: return "Em Integração";
                case SituacaoCancelamentoPagamento.FalhaIntegracao: return "Falha na integração";
                case SituacaoCancelamentoPagamento.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
