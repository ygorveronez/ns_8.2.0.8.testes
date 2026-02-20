namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class MontagemCargaGrupoPedidoProduto
    {
        public int CodigoPedido { get; set; }
        public int CodigoPedidoProduto { get; set; }
        public decimal PesoPedidoProduto { get; set; }
        public decimal QuantidadePedidoProduto { get; set; }
        public decimal QuantidadePalletPedidoProduto { get; set; }
        public decimal MetroCubicoPedidoProduto { get; set; }
        public int CodigoLinhaSeparacao { get; set; }
    }
}
