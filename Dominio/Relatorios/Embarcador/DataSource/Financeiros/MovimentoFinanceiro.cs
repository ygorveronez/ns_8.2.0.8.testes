using System;


namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class MovimentoFinanceiro
    {
        #region Propriedades 
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataBaseFinanceiro { get; set; }
        public decimal ValorMovimento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Observacao { get; set; }
        public string TipoMovimento { get; set; }
        public string CentroResultado { get; set; }
        public string PlanoDebito { get; set; }
        public string PlanoCredito { get; set; }
        public string Pessoa { get; set; }
        public string GrupoPessoa { get; set; }
        public string Usuario { get; set; }
        public int SituacaoMovimento { get; set; }
        public string ContaPlanoDebito { get; set; }
        public int CodigoPlanoDebito { get; set; }
        public string ContaPlanoCredito { get; set; }
        public int CodigoPlanoCredito { get; set; }
        public string ContaFornecedorEBS { get; set; }

        #endregion

    }
}