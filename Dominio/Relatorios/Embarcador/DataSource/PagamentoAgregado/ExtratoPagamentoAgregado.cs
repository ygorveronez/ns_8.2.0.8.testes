namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado
{
    public class ExtratoPagamentoAgregado
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public string CPFCNPJ { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Cavalo { get; set; }
        public string VeiculosVinculados { get; set; }
        public string Motorista { get; set; }
        public decimal Saldo { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string ContaCorrente { get; set; }
        public string PIX { get; set; }
        public int CodigoPagamento { get; set; }
    }
}
