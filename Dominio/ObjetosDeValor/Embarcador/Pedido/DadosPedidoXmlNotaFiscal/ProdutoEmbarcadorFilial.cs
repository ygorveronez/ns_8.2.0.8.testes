namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class ProdutoEmbarcadorFilial
    {
        public int Codigo { get; set; }

        public int CodigoProdutoEmbarcador { get; set; }

        public Filial Filial { get; set; }

        public ProdutoEmbarcadorFilialSituacoes FilialSituacao { get; set; }

        public Enumeradores.UsoMaterial UsoMaterial { get; set; }
    }
}
