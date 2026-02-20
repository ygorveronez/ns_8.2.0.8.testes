namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class ProdutoEmbarcadorFornecedor
    {
        public int Codigo { get; set; }

        public int CodigoProdutoEmbarcador { get; set; }

        public string CodigoInterno { get; set; }

        public Filial Filial { get; set; }

        public Cliente Fornecedor { get; set; }
    }
}
