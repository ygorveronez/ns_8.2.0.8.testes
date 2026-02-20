namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class CargaPedidoProduto
    {
        public int Codigo { get; set; }

        public int CodigoCargaPedido { get; set; }

        public ProdutoEmbarcador ProdutoEmbarcador { get; set; }
    }
}
