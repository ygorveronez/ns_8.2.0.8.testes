using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ModeloBMovimentoFinanceiro
    {
        public string Vendor { get; set; }
        public string Name { get; set; }
        public decimal TotalValue { get; set; }
        public string Reference { get; set; }
        public decimal AmountInLC { get; set; }
        public DateTime? Clearing { get; set; }
        public string ClearingDocument { get; set; }
        public string MaterialDocument { get; set; }
        public string PurchasingDocument { get; set; }
        public string DocumentType { get; set; }
    }
}
