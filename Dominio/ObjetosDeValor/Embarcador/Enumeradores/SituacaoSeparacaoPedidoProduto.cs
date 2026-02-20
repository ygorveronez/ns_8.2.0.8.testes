namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSeparacaoPedidoProduto
    {
        Disponivel = 1,
        EmPreparacao = 2,
        SemEstoque = 8
    }

    public static class SituacaoProdutoEmbarcadorHelper
    {
        public static string ObterDescricao(this SituacaoSeparacaoPedidoProduto situacao)
        {
            switch (situacao)
            {
                case SituacaoSeparacaoPedidoProduto.Disponivel: return "Disponível";
                case SituacaoSeparacaoPedidoProduto.EmPreparacao: return "Em Preparação";
                case SituacaoSeparacaoPedidoProduto.SemEstoque: return "Sem Estoque";
                default: return string.Empty;
            }
        }

        public static SituacaoSeparacaoPedidoProduto SituacaoPadrao()
        {
            return SituacaoSeparacaoPedidoProduto.Disponivel;
        }
    }
}
