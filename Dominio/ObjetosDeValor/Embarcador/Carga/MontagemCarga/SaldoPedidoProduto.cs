namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SaldoPedidoProduto
    {
        public int CodigoPedido { get; set; }
        public int CodigoPedidoProduto { get; set; }
        public bool PalletFechado { get; set; }
        public string Cliente { get; set; }
        public int CodigoProduto { get; set; }
        public string CodigoProdutoEmbarcador { get; set; }
        public string Produto { get; set; }
        public string Categoria { get; set; }
        public string LinhaSeparacao { get; set; }
        public string GrupoProduto { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string IdDemanda { get; set; }
        /// <summary>
        /// Contem a quantidade total do produto do pedido
        /// </summary>
        public decimal Qtde { get; set; }
        /// <summary>
        /// Comtem o saldo do produto do pedido (Qtde - Carregado)
        /// </summary>
        public decimal SaldoQtde { get { return this.Qtde - this.QtdeCarregado; } }
        public decimal Peso { get; set; }
        public decimal SaldoPeso { get { return this.Peso - this.PesoCarregado; } }
        public decimal Pallet { get; set; }
        public decimal SaldoPallet { get; set; }
        public decimal Metro { get; set; }
        public decimal SaldoMetro { get; set; }
        
        /// <summary>
        /// Necessário pois o Portal Retira possui saldo furado e o método AlterarSaldoProdutoPedido do webservice Pedido vai corrigir.
        /// </summary>
        public decimal QtdeCarregado { get; set; }
        public decimal PesoCarregado { get; set; }
        public string ObservacaoProduto { get; set; }
        public string TipoEmbalagem {  get; set; }
    }
}
