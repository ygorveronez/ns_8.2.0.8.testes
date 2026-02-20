using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class HistoricoProduto
    {
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public string Pessoa { get; set; }
        public DateTime Data { get; set; }
        public string DescricaoTipo { get; set; }
        public Int64 Numero { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Valor { get; set; }
        public string Empresa { get; set; }
        public string GrupoProduto { get; set; }

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string QuantidadeFormatada
        {
            get { return Quantidade.ToString("n4"); }
        }
    }
}
