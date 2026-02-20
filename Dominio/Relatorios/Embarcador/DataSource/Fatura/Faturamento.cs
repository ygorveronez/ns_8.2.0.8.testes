using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fatura
{
    public class Faturamento
    {
        public int NumeroFatura { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataQuitacao { get; set; }
        public string Grupo { get; set; }
        public string Pessoa { get; set; }
        public decimal ValorFatura { get; set; }
        public decimal TotalAcrescimos { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal TotalFatura { get; set; }
        public decimal SaldoAberto { get; set; }
        public string StatusFinanceiro { get; set; }
        public int CodigoTitulo { get; set; }
        public int Sequencia { get; set; }
        public decimal ValorPendente { get; set; }
        public decimal ValorPago { get; set; }
        public int CodigoFatura { get; set; }
        public DateTime DataBaseQuitacao { get; set; }
        public string Conhecimentos { get; set; }
        public string Cargas { get; set; }
    }
}
