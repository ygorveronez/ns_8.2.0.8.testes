namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPedidoVenda
    {
        Todos = -1,
        Cotacao = 1,
        Pedido = 2,
        OrdemServico = 3,
        OrdemServicoPet = 4
    }

    public static class TipoPedidoVendaHelper
    {
        public static string ObterDescricao(this TipoPedidoVenda tipo)
        {
            switch (tipo)
            {
                case TipoPedidoVenda.Cotacao: return "Cotação";
                case TipoPedidoVenda.Pedido: return "Pedido";
                case TipoPedidoVenda.OrdemServico: return "Ordem de Serviço";
                case TipoPedidoVenda.OrdemServicoPet: return "Ordem de Serviço de Pet";
                default: return string.Empty;
            }
        }
    }
}
