namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class OrdemColetaPedidoProduto
    { 
        #region Produto

        public int CodigoPedido { get; set; }
        public string Item { get; set; }
        public string Produto { get; set; }
        public string Lote { get; set; }
        public string PadraoProduto { get; set; }
        public string Quantidade { get; set; }
        public string PesoLiquido { get; set; }
        public string Compartimento { get; set; }

        #endregion
    }
}
