namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoParaDuplicacao
    {
        public Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedidoColeta { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoNotaFiscal { get; set; }
        public Dominio.Entidades.Cliente Destinatario { get; set; }
        public Dominio.Entidades.Localidade Destino { get; set; }
    }
}
