using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class ProdutoSemMovimentacao
    {
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public decimal Estoque { get; set; }
        public decimal Preco { get; set; }
        public decimal CustoMedio { get; set; }
        public string DescricaoStatus { get; set; }
        public DateTime DataUltimaCompra { get; set; }
        public DateTime DataUltimaVenda { get; set; }
        public string Empresa { get; set; }
        public string GrupoProduto { get; set; }

        public string DataUltimaCompraFormatada
        {
            get { return DataUltimaCompra != DateTime.MinValue ? DataUltimaCompra.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataUltimaVendaFormatada
        {
            get { return DataUltimaVenda != DateTime.MinValue ? DataUltimaVenda.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string EstoqueFormatado
        {
            get { return Estoque.ToString("n4"); }
        }
    }
}
