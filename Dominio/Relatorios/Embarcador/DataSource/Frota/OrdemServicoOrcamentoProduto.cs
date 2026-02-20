namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class OrdemServicoOrcamentoProduto
    {
        public int CodigoServico { get; set; }
        public int OrdemServicoFrotaOrcamentoServico { get; set; }
        public string Produto { get; set; }
        public decimal ValorProduto { get; set; }
        public decimal QuantidadeProduto { get; set; }
        public bool Garantia { get; set; }
        public bool Autorizado { get; set; }

        public decimal ValorTotal
        {
            get { return QuantidadeProduto * ValorProduto; }
        }
    }
}
