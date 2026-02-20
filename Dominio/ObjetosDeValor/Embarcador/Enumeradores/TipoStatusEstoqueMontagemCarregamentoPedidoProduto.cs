namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoStatusEstoqueMontagemCarregamentoPedidoProduto
    {
        Ambos = 0,
        EstoqueParcial = 1,
        EstoqueTotal = 2,
    }

    public static class TipoStatusEstoqueMontagemCarregamentoPedidoProdutoHelper
    {
        public static string ObterDescricao(this TipoStatusEstoqueMontagemCarregamentoPedidoProduto tipoStatusEstoqueMontagemCarregamentoPedidoProduto)
        {
            return tipoStatusEstoqueMontagemCarregamentoPedidoProduto switch
            {
                TipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos => Localization.Resources.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos,
                TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueParcial => Localization.Resources.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueParcial,
                TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueTotal => Localization.Resources.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueTotal,
                _ => string.Empty,
            };
        }
    }
}
