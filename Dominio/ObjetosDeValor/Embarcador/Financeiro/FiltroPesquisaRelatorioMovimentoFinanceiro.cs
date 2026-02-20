using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioMovimentoFinanceiro
    {
        public int Codigo { get; set; }
        public DateTime DataMovimentoInicial { get; set; }
        public DateTime DataMovimentoFinal { get; set; }
        public DateTime DataBaseFinanceiro { get; set; }
        public decimal ValorMovimento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Observacao { get; set; }
        public int TipoMovimento { get; set; }
        public int CentroResultado { get; set; }
        public int PlanoDebito { get; set; }
        public int PlanoCredito { get; set; }
        public int Pessoa { get; set; }
        public int GrupoPessoa { get; set; }
        public bool VisualizarTitulosPagamentoSalario { get; set; }
    }
}