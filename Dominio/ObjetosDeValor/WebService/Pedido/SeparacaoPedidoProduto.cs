namespace Dominio.ObjetosDeValor.WebService.Pedido
{
   public sealed class SeparacaoPedidoProduto
    {
        /// <summary>
        /// CodigoProdutoEmbarcador no Produto
        /// </summary>
        public string ProtocoloProduto { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedidoProduto Situacao { get; set; }
    }
}
