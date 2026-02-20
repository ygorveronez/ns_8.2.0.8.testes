namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPedidoGestaoPedido
    {
        Todos = 0,
        PedidoSemCarga = 1,
        PedidoEmSessao = 2,
        PedidoComSaldoDisponivel = 3,       
    }

    public static class SituacaoPedidoGestaoPedidoHelper
    {
        public static string ObterDescricao(this SituacaoPedidoGestaoPedido situacaoPedido)
        {
            switch (situacaoPedido)
            {
                case SituacaoPedidoGestaoPedido.Todos:
                    return "Todos";
                case SituacaoPedidoGestaoPedido.PedidoSemCarga:
                    return "Pedido sem carga";
                case SituacaoPedidoGestaoPedido.PedidoEmSessao:
                    return "Pedido em sessão";
                case SituacaoPedidoGestaoPedido.PedidoComSaldoDisponivel:
                    return "Pedido com saldo disponível";                
                default:
                    return string.Empty;
            }
        }
    }
}
