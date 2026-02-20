using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class AbastecimentoNotaEntrada
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Veiculo { get; set; }
        public string Posto { get; set; }
        public string Produto { get; set; }
        public int KM { get; set; }
        public decimal Litros { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public Int64 NumeroNF { get; set; }
        public DateTime DataNF { get; set; }
        public decimal QuantidadeNF { get; set; }
        public decimal ValorUnitarioNF { get; set; }
        public decimal ValorTotalNF { get; set; }
    }
}