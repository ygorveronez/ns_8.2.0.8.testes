using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ModeloCMovimentoFinanceiro
    {
        public string AccessKey { get; set; }
        public DateTime? PostingDate { get; set; }
        public string CteNumber { get; set; }
        public string CteSeries { get; set; }
        public string CompanyCode { get; set; }
        public string CteStatus { get; set; }
        public decimal NetAmount { get; set; }
        public decimal BaseAmount { get; set; }
        public string Currency { get; set; }
        public string ProtocolNumber { get; set; }
        public string InvoiceDocumentNumber { get; set; }
        public string CNPJIssuer { get; set; }
        public string IssuerName { get; set; }
        public string Vendor { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string CNPJSender { get; set; }
        public string SenderName { get; set; }
        public string CNPJReceiver { get; set; }
        public string ReceiverName { get; set; }
        public string CNPJDispatcher { get; set; }
        public string DispatcherName { get; set; }
        public string CNPJReceipt { get; set; }
        public string ReceiptName { get; set; }
        public DateTime? DueDate { get; set; }
        public string LogMessage { get; set; }
        public DateTime? UploadDate { get; set; }
        public string Comments { get; set; }
        public DateTime? TaxApprovalDate { get; set; }
        public string Comment { get; set; }
    }
}
