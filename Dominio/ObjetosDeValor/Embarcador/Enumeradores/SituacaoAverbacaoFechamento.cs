namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAverbacaoFechamento
    {
        Todas = 0,
        EmAberto = 1,
        EmFechamento = 2,
        Finalizada = 3
    }

    public static class SituacaoAverbacaoFechamentoHelper
    {
        public static string ObterDescricao(this SituacaoAverbacaoFechamento situacao)
        {
            switch (situacao)
            {
                case SituacaoAverbacaoFechamento.EmAberto: return "Em Aberto";
                case SituacaoAverbacaoFechamento.EmFechamento: return "Em Fechamento";
                case SituacaoAverbacaoFechamento.Finalizada: return "Finalizada";
                default: return string.Empty;
            }
        }
    }
}
