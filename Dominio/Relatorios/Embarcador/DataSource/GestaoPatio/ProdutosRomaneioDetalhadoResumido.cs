namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class ProdutosRomaneioDetalhadoResumido
    {
        public string ProdutoCodigo { get; set; }
        public string ProdutoDescricao { get; set; }
        public decimal ProdutoQuantidade { get; set; }
        public decimal QuantidadePallet { get; set; }
        public int PedidoCodigo { get; set; }
        public double CodigoDestinatario { get; set; }
        public string PedidoNumero { get; set; }
    }
}
