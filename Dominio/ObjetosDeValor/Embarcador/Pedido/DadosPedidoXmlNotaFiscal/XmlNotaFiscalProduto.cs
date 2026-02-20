namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class XmlNotaFiscalProduto
    {
        public int Codigo { get; set; }

        public string CodigoProduto { get; set; }
        public string NumeroPedidoCompra { get; set; }

        public int CodigoXmlNotaFiscal { get; set; }

        public ProdutoEmbarcador ProdutoEmbarcador { get; set; }
    }
}
