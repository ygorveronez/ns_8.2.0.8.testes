using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class NotaEntradaOrdemCompra
    {
        public Int64 NumeroNota { get; set; }

        public int NumeroOrdem { get; set; }

        public string Fornecedor { get; set; }

        public DateTime DataEntrada { get; set; }

        public string Produto { get; set; }

        public decimal QuantidadeNF { get; set; }

        public decimal QuantidadeOrdem { get; set; }

        public decimal ValorNota { get; set; }

        public decimal ValorOrdem { get; set; }

        public string DataEntradaFormatada
        {
            get { return DataEntrada != DateTime.MinValue ? DataEntrada.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
