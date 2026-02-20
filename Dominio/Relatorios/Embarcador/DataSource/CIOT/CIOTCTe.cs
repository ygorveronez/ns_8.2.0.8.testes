namespace Dominio.Relatorios.Embarcador.DataSource.CIOT
{
    public class CIOTCTe
    {
        public int CodigoCTe { get; set; }
        public string NumeroCTe { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string Mercadoria { get; set; }
        public decimal Quantidade { get; set; }
        public string Especie { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal Adiantamento { get; set; }
        public decimal Seguro { get; set; }
        public decimal Pedagio { get; set; }
        public decimal Tarifa { get; set; }
        public decimal IRRF { get; set; }
        public decimal INSS { get; set; }
        public decimal SEST { get; set; }
        public decimal SENAT { get; set; }
        public decimal Descontos { get; set; }
    }
}
