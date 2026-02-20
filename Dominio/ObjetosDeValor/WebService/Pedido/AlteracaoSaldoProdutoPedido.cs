namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class AlteracaoSaldoProdutoPedido
    {
        /// <summary>
        /// CodigoProdutoEmbarcador no Produto
        /// </summary>
        public string ProtocoloProduto { get; set; }

        public decimal NovoSaldo { get; set; }
    }
}
