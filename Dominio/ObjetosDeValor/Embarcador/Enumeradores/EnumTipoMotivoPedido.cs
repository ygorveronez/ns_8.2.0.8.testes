namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumTipoMotivoPedido
    {
        LancamentoPedido = 0,
        AprovacaoPedido = 1,
        RejeicaoPedido = 2
    }

    public static class TipoMotivoPedidoHelper
    {
        public static string ObterDescricao(this EnumTipoMotivoPedido tipo)
        {
            switch (tipo)
            {
                case EnumTipoMotivoPedido.LancamentoPedido: return "Lançamento de Pedido";
                case EnumTipoMotivoPedido.AprovacaoPedido: return "Aprovação de Pedido";
                case EnumTipoMotivoPedido.RejeicaoPedido: return "Rejeição de Pedido";
                default: return string.Empty;
            }
        }
    }
}
