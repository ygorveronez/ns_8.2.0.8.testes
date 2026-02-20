using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class CotacaoCompra
    {
        public int Codigo { get; set; }
        public int NumeroCotacao { get; set; }
        public string DescricaoCotacao { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Fornecedor { get; set; }
        public string Produto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadeRetornado { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorUnitarioRetornado { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorTotalRetornado { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
