using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ConciliacaoBancaria
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public string Documento { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorCredito { get; set; }
        public int CodigoPlanoConta { get; set; }
        public string PlanoConta { get; set; }
        public string PlanoContaDescricao { get; set; }
        public string TipoMovimento { get; set; }

        public string DescricaoData
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
