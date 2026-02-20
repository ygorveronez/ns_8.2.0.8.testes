namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLiberacaoPagamentoProvedor
    {
        Todos = 0,
        Aberto = 1,
        Rejeitada = 2,
        Finalizada = 3,
        Cancelada = 4
    }

    public static class SituacaoProvedorHelper
    {
        public static string ObterDescricao(this SituacaoLiberacaoPagamentoProvedor etapa)
        {
            switch (etapa)
            {
                case SituacaoLiberacaoPagamentoProvedor.Todos: return "Todos";
                case SituacaoLiberacaoPagamentoProvedor.Aberto: return "Aberto";
                case SituacaoLiberacaoPagamentoProvedor.Rejeitada: return "Rejeitada";
                case SituacaoLiberacaoPagamentoProvedor.Finalizada: return "Finalizada";
                case SituacaoLiberacaoPagamentoProvedor.Cancelada: return "Cancelada";
                default: return string.Empty;
            }
        }
    }
}
