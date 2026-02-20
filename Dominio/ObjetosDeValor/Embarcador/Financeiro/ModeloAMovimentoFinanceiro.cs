using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ModeloAMovimentoFinanceiro
    {
        public string CompanyCode { get; set; }
        public string Vendor { get; set; }
        public string Reference { get; set; }
        public string PmntBlock { get; set; }
        public string PostingKey { get; set; }
        public string TaxCode { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string DocumentType { get; set; }
        public decimal AmountInLC { get; set; }
        public string SpecialGLIndicator { get; set; }
        public DateTime? Clearing { get; set; }
        public string ClearingDocument { get; set; }
        public string Text { get; set; }
        public string DocumentNumber { get; set; }
        public string UserName { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime? PostingDate { get; set; }
        public string Assignment { get; set; }
        public decimal DiscountBase { get; set; }
        public string Currency { get; set; }
        public string PaymentTerms { get; set; }
        public string DocumentHeaderText { get; set; }
        public string ReferenceKey { get; set; }
    }
}
