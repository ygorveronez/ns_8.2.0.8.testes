namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioPedidoOrdemVendaItens
    {
        public int CodigoPedido { get; set; }
        public string CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public decimal QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }
    }
}
