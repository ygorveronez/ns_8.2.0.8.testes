namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class OrdemDeCompraComparacao
    {
        public string CodigoDocumentoPO { get; set; }
        public double CNPJRemetente { get; set; }
        public double CNPJDestinatario { get; set; }
        public OrdemDeCompraComparacaoItem Item { get; set; }
    }

    public  class OrdemDeCompraComparacaoItem
    {
        public string CodigoProdutoEmbarcador { get; set; }
        public int CodigoProduto { get; set; }
        public decimal Quantidade { get; set; }
        public bool Chaveamento { get; set; }
        public decimal Tolerancia { get; set; }
        public decimal TotalDebitos { get; set; }
        public decimal TotalCreditos { get; set; }
    }
}
