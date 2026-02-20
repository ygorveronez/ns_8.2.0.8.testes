using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ModeloDMovimentoFinanceiro
    {
        public string Name { get; set; }
        public string Vendor { get; set; }
        public string CompanyCode { get; set; }
        public string TradingPartner { get; set; }
        public string GLAccount { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string ProfitCenter { get; set; }
        public string Text { get; set; }
        public string Segment { get; set; }
        public string Order { get; set; }
        public string CostCenter { get; set; }
        public string Reference { get; set; }
        public decimal AmountInLC { get; set; }
        public string UserName { get; set; }
        public string TaxCode { get; set; }
        public string Currency { get; set; }
        public string DocumentNumber { get; set; }
        public string ClearingDocument { get; set; }
        public string DebitCredit { get; set; }
    }
}
