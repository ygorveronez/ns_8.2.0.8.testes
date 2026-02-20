using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class DespesaMensal
    {
        public int Codigo { get; set; }
        public string Pessoa { get; set; }
        public string Descricao { get; set; }
        public int DiaProvisao { get; set; }
        public decimal ValorProvisao { get; set; }
        public string TipoDespesa { get; set; }
        public DateTime DataGeracao { get; set; }
        public int CodigoTitulo { get; set; }
        public decimal ValorTitulo { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
