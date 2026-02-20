namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo
{
    public class GestaoDevolucaoLaudoProduto
    {
        public long CodigoLaudo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public decimal QuantidadeOrigem { get; set; }
        public decimal TotalQuantidadeOrigem { get; set; }
        public decimal QuantidadeDevolvida { get; set; }
        public decimal TotalQuantidadeDevolvida { get; set; }
        public decimal QuantidadeAvariada { get; set; }
        public decimal TotalQuantidadeAvariada { get; set; }
        public decimal QuantidadeFalta { get; set; }
        public decimal TotalQuantidadeFalta { get; set; }
        public decimal ValorAvariado { get; set; }
        public decimal TotalValorAvariado { get; set; }
        public decimal ValorFalta { get; set; }
        public decimal TotalValorFalta { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal TotalValor { get; set; }
        public decimal ValorSemCondicao { get; set; }
        public decimal TotalValorSemCondicao { get; set; }
        public decimal QuantidadeSemCondicao { get; set; }
        public decimal TotalQuantidadeSemCondicao { get; set; }
    }
}
