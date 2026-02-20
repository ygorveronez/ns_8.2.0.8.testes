namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class IntegracaoProdutos
    {
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string CodigoGrupoProduto { get; set; }
        public string DescricaoGrupoProduto { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadeEmbalagem { get; set; }
        public decimal PesoTotalEmbalagem { get; set; }

    }
}
