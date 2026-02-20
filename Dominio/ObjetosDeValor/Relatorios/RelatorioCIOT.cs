namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCIOT
    {
        public int Codigo { get; set; }
        public int NumeroViagem { get; set; }
        public string NumeroCIOT { get; set; }
        public string DataEmissao { get; set; }
        public string Motorista { get; set; }
        public string Veiculo { get; set; }
        public double CNPJ_Contratado { get; set; }
        public string Contratado { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal Impostos { get; set; }
        public decimal SeguroPedagio { get; set; }
        public decimal Adiantamento { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal TotalOperacao { get; set; }
        public decimal TotalQuitacao { get; set; }
        public string Status { get; set; }
    }
}
