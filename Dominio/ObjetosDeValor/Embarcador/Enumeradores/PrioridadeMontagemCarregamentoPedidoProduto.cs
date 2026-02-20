namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PrioridadeMontagemCarregamentoPedidoProduto
    {
        CanalEntregaLinhaSeparacaoPedido = 0,
        LinhaSeparacaoPedidoCanalEntrega = 1,
        CanalEntregaLinhaSeparacaoProduto = 2,
        LinhaSeparacaoCanalEntregaProduto = 3,
        EnderecoProdutoDataPedido = 4,
        CanalEntregaEnderecoProdutoDataPedido = 5
    }

    public static class PrioridadeMontagemCarregamentoPedidoProdutoHelper
    {
        public static string ObterDescricao(this PrioridadeMontagemCarregamentoPedidoProduto prioridade)
        {
            switch (prioridade)
            {
                case PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido: return "Canal entrega, Linha de separação e Pedido";
                case PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoPedidoCanalEntrega: return "Linha de separação, Canal entrega e Pedido";
                case PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto: return "Canal entrega, Linha de separação e Produto";
                case PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoCanalEntregaProduto: return "Linha de separação, Canal entrega e Produto";
                case PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido: return "Endereço Produto e Data do Pedido";
                case PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido: return "Canal entrega, Endereço Produto e Data do Pedido";
                default: return string.Empty;
            }
        }
    }

    public enum PrioridadeMontagemCarregamentoPedido
    {
        CanalEntregaPrevisaoEntrega = 0,
        PrevisaoEntregaCanalEntrega = 1
    }

    public static class PrioridadeMontagemCarregamentoPedidoHelper
    {
        public static string ObterDescricao(this PrioridadeMontagemCarregamentoPedido prioridade)
        {
            switch (prioridade)
            {
                case PrioridadeMontagemCarregamentoPedido.CanalEntregaPrevisaoEntrega: return "Canal entrega, Previsão entrega pedido";
                case PrioridadeMontagemCarregamentoPedido.PrevisaoEntregaCanalEntrega: return "Previsão entrega pedido, Canal entrega";
                default: return string.Empty;
            }
        }
    }
}
