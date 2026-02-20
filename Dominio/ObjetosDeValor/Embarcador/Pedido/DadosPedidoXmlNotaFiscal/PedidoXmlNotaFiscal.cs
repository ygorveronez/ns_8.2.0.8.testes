namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class PedidoXmlNotaFiscal
    {
        public int Codigo { get; set; }

        public Carga Carga { get; set; }

        public CargaPedido CargaPedido { get; set; }

        public XmlNotaFiscal XmlNotaFiscal { get; set; }
    }
}
