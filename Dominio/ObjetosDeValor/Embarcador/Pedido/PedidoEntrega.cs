namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoEntrega
    {
        public PedidoEntregaProtocolo Protocolo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido Situacao { get; set; }

        public string DataConfirmacao { get; set; }
    }
}
