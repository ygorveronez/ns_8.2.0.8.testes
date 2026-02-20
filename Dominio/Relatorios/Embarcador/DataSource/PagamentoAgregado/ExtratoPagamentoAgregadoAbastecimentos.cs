namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado
{
    public class ExtratoPagamentoAgregadoAbastecimentos
    {
        public int CodigoPagamento { get; set; }
        public int Sequencia { get; set; }
        public string Data { get; set; }
        public string Descricao { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public decimal Saldo { get; set; }
    }
}
