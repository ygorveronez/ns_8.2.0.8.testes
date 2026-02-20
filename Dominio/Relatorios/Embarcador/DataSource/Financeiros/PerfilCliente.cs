using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class PerfilCliente
    {
        public double CNPJPessoa { get; set; }
        public string Pessoa { get; set; }
        public string Telefone { get; set; }
        public decimal MediaCompras { get; set; }
        public decimal MaiorCompra { get; set; }
        public decimal UltimaCompra { get; set; }
        public DateTime DataUltimaCompra { get; set; }
        public DateTime VencimentoUltimaCompra { get; set; }
        public DateTime ProximoVencimento { get; set; }
        public decimal ValorProximoVencimento { get; set; }
        public DateTime DataPrimeiraCompra { get; set; }
        public decimal TotalPago { get; set; }
        public decimal TotalGeral { get; set; }
        public decimal TotalPendente { get; set; }
        public decimal TotalVencer { get; set; }
        public decimal TotalVencido { get; set; }
        public int QuantidadeTitulos { get; set; }
    }
}
