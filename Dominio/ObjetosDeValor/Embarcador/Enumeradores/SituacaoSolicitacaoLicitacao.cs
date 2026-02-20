namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSolicitacaoLicitacao
    {
        Todos = 0,
        AgCotacao = 1,
        Finalizada = 2,
        Rejeitada = 3,
        Cancelada = 4
    }

    public static class SituacaoSolicitacaoLicitacaoHelper
    {
        public static string ObterDescricao(this SituacaoSolicitacaoLicitacao situacao)
        {
            switch (situacao)
            {
                case SituacaoSolicitacaoLicitacao.AgCotacao: return "Ag. Cotação";
                case SituacaoSolicitacaoLicitacao.Finalizada: return "Finalizada";
                case SituacaoSolicitacaoLicitacao.Rejeitada: return "Rejeitada";
                case SituacaoSolicitacaoLicitacao.Cancelada: return "Cancelada";
                default: return string.Empty;
            }
        }
    }
}
