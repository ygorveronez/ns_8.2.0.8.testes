namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoProduto
    {
        public string Produto { get; set; }

        public decimal Quantidade { get; set; }

        public decimal QuantidadePlanejada { get; set; }
        
        public decimal PesoUnitario { get; set; }

        public decimal PesoTotalEmbalagem { get; set; }

        public decimal Valor { get; set; }
        
        public decimal ValorUnitario { get; set; }

        public string NumeroLotePedidoProdutoLote { get; set; }
        public string LinhaSeparacao { get; set; }

        public string Lote { get; set; }

        public decimal PesoTotalProduto { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal PesoTotal
        {
            get
            {
                return  (Quantidade * PesoUnitario) + PesoTotalEmbalagem;
            }
        }
    }
}
